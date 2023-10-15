const apiAuthUrl = 'https://localhost:7205/api/Authentication/Login'

// Metod som skickar en post request till API:ets Login endpoint
// Returnerar true och sätter token i localstorage ifall requesten lyckades, returnerar annars false
const login = async (identityName) => {
    localStorage.removeItem('authToken')

    const loginRequest = {
        name: identityName
    }

    const res = await fetch(apiAuthUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginRequest)
    })

    if (res.status === 200) {
        const token = await res.text()
        localStorage.setItem('authToken', token)
        return true
    }

    return false
}

// Tar bort den sparade tokenen från localstorage
const logout = () => {
    localStorage.removeItem('authToken')
}