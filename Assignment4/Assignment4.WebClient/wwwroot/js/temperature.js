'use strict'

let authorized = false
let deviceLists = {}
let showLog = false

// Auktoriserar som 'user' till API:et vid klick
document.getElementById('loginBtn').addEventListener('click', () => {
  const statusText = document.getElementById('statusText')
  statusText.textContent = 'Authorizing...'
  login('user')
})

// Loggar ut och stoppar anslutningen vid klick
document.getElementById('logoutBtn').addEventListener('click', () => {
  logout()
  connection.stop()
})

// Ändrar layout när auktoriserngsvärdet ändras
const setAuthorized = (value) => {
  const statusText = document.getElementById('statusText')
  const loginBtn = document.getElementById('loginBtn')
  const logoutBtn = document.getElementById('logoutBtn')
  statusText.style.display = 'block'

  authorized = value

  if (!authorized) {
    loginBtn.disabled = false
    logoutBtn.disabled = true
    statusText.textContent = 'Not authorized, login to retrieve values'
    return
  }

  loginBtn.disabled = true
  logoutBtn.disabled = false
  statusText.textContent = 'Loading...'
}

// Bygger upp en anslutning till serverns Hub och anger som token den som sparats i localstorage
const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7205/temperatureHub', {
    accessTokenFactory: () => {
      return localStorage.getItem('authToken')
    }
  })
  .withAutomaticReconnect()
  .build()

// Startar anslutning, ifall att det ej lyckas väntar metoden 5 sekunder för att sedan försöka starta igen
const startConnection = async () => {
  try {
    await connection.start();
    setAuthorized(true)
  } catch (err) {
    console.error(err)
    setTimeout(startConnection, 5000)

    if (err.message.includes("Status code '401'")) {
      setAuthorized(false)
    }
  }
}

// Ifall anslutningen upphörs, starta startloopen igen
connection.onclose(async () => {
  await startConnection()
})

// Starta anslutning
startConnection()

// När anslutningen tar emot data från Hubmetoden 'ReceiveTemperatureData' valideras först datan och avbryter ifall datan ej är giltig
// Resten av koden är bara till för att visa/uppdatera datan i de relevanta HTML-elementen
connection.on('ReceiveTemperatureData', function (temperatureData) {
  if (!isValidTemperatureData(temperatureData)) {
    console.error('Invalid temperature data')
    return
  }

  document.getElementById('statusText').style.display = 'none'

  if (!deviceLists.hasOwnProperty(temperatureData.deviceId)) {
    deviceLists[temperatureData.deviceId] = []
  }

  deviceLists[temperatureData.deviceId].push(temperatureData)

  const contentBoxId = `iotContent-${temperatureData.deviceId}`

  let iotNameHeader
  let iotTemperatureText
  let iotHumidityText
  let iotDateSentText
  let showLogElement

  var contentBox = document.getElementById(contentBoxId)

  if (!contentBox) {
    contentBox = document.createElement('div')
    contentBox.className = 'iot-content-box'
    contentBox.id = contentBoxId

    iotNameHeader = document.createElement('h3')
    iotNameHeader.id = `${temperatureData.deviceId}-Header`

    iotTemperatureText = document.createElement('p')
    iotTemperatureText.id = `${temperatureData.deviceId}-Temperature`

    iotHumidityText = document.createElement('p')
    iotHumidityText.id = `${temperatureData.deviceId}-Humidity`

    iotDateSentText = document.createElement('p')
    iotDateSentText.id = `${temperatureData.deviceId}-DateSent`

    showLogElement = document.createElement('u')
    showLogElement.id = `${temperatureData.deviceId}-ShowLog`
    showLogElement.textContent = 'Show log'
    showLogElement.className = 'showLog'

    showLogElement.addEventListener('click', () => {
      if (showLog === true) {
        showLog = false
      } else {
        showLog = true
      }
      document.getElementById(`temperatureList-${temperatureData.deviceId}`).style.display = showLog ? 'block' : 'none'
    })

    contentBox.append(
      iotNameHeader,
      iotTemperatureText,
      iotHumidityText,
      iotDateSentText,
      showLogElement
    )

    document.querySelector('.iot-container').appendChild(contentBox)
  }

  document.getElementById(`${temperatureData.deviceId}-Header`).textContent = `IoT Device: ${temperatureData.deviceId}`
  document.getElementById(`${temperatureData.deviceId}-Temperature`).textContent = `${temperatureData.temperature.toFixed(1)} C`
  document.getElementById(`${temperatureData.deviceId}-Humidity`).textContent = `H: ${temperatureData.humidity} %`
  document.getElementById(`${temperatureData.deviceId}-DateSent`).textContent = `${formatDate(new Date(temperatureData.dateSent))}`

  const ulId = `temperatureList-${temperatureData.deviceId}`
  const li = document.createElement('li')
  let ul = document.getElementById(ulId)

  if (!ul) {
    ul = document.createElement('ul')
    ul.id = ulId
    ul.className = 'temperatureLog'
    document.getElementById(`${contentBox.id}`).appendChild(ul)
  }

  li.textContent = `${temperatureData.dateSent} - Device ${temperatureData.deviceId
    }: ${temperatureData.temperature.toFixed(1)} C | H: ${temperatureData.humidity
    }`
  ul.appendChild(li)
})

// Formaterar ett datum för att få det mer läsbart
function formatDate(date) {
  const options = {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hour12: false,
  };
  return date.toLocaleDateString(undefined, options);
}

// Validerar temperaturdata genom att kolla att värdena är av rätt typ.
function isValidTemperatureData(temperatureData) {
  let isValid = true

  if (typeof temperatureData.temperature !== 'number' || isNaN(temperatureData.temperature)) {
    isValid = false
  }

  if (typeof temperatureData.humidity !== 'number' || isNaN(temperatureData.humidity)) {
    isValid = false
  }

  if (typeof temperatureData.deviceId !== 'string') {
    isValid = false
  }

  if (!isValidDate(temperatureData.dateSent)) {
    isValid = false
  }

  return isValid
}

// Validerar ett datum med hjälp av Regex
function isValidDate(dateString) {
  const dateRegex = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+(?:[+-]\d{2}:\d{2})$/
  return dateRegex.test(dateString)
}