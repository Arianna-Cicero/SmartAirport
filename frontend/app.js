// API Configuration
const API_BASE_URL = 'https://smartairport-flightservice.azurewebsites.net';
const SENSOR_API_URL = 'https://smartairport-sensorservice.azurewebsites.net';
const EXTERNAL_API_URL = 'https://smartairport-externalservice.azurewebsites.net';

let authToken = null;
let currentUser = null;

// Show message
function showMessage(text, isError = false) {
    const msg = document.getElementById('message');
    msg.textContent = text;
    msg.className = isError ? 'message error' : 'message';
    msg.style.display = 'block';
    setTimeout(() => msg.style.display = 'none', 3000);
}

// Login
async function login() {
    const username = document.getElementById('login-username').value;
    const password = document.getElementById('login-password').value;

    if (!username || !password) {
        showMessage('Please enter username and password', true);
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username: username, Password: password })
        });

        if (response.ok) {
            const data = await response.json();
            authToken = data.token;
            currentUser = username;
            
            document.getElementById('login-section').style.display = 'none';
            document.getElementById('main-section').style.display = 'block';
            document.getElementById('user-info').style.display = 'flex';
            document.getElementById('username').textContent = `Welcome, ${username}!`;
            
            showMessage('Login successful!');
            loadFlights();
        } else {
            showMessage('Invalid credentials', true);
        }
    } catch (error) {
        showMessage('Login failed: ' + error.message, true);
    }
}

// Logout
function logout() {
    authToken = null;
    currentUser = null;
    document.getElementById('login-section').style.display = 'block';
    document.getElementById('main-section').style.display = 'none';
    document.getElementById('user-info').style.display = 'none';
    document.getElementById('login-username').value = '';
    document.getElementById('login-password').value = '';
}

// Tab navigation
function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
    
    document.getElementById(`${tabName}-tab`).classList.add('active');
    event.target.classList.add('active');
    
    // Auto-load data when switching tabs
    switch(tabName) {
        case 'flights': loadFlights(); break;
        case 'airports': loadAirports(); break;
        case 'airlines': loadAirlines(); break;
        case 'passengers': loadPassengers(); break;
    }
}

// Load Flights
async function loadFlights() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/flight`);
        const flights = await response.json();
        
        const list = document.getElementById('flights-list');
        if (flights.length === 0) {
            list.innerHTML = '<p>No flights found</p>';
        } else {
            list.innerHTML = flights.map(f => `
                <div class="data-item">
                    <strong>${f.flightno}</strong> - 
                    From: ${f.from} To: ${f.to} | 
                    Departure: ${new Date(f.departure).toLocaleString()}
                </div>
            `).join('');
        }
    } catch (error) {
        showMessage('Failed to load flights: ' + error.message, true);
    }
}

// Create Flight
async function createFlight() {
    const flightNo = document.getElementById('flight-no').value;
    const from = parseInt(document.getElementById('flight-from').value);
    const to = parseInt(document.getElementById('flight-to').value);
    const departure = document.getElementById('flight-departure').value;
    const arrival = document.getElementById('flight-arrival').value;
    const airlineId = parseInt(document.getElementById('flight-airline').value);
    const airplaneId = parseInt(document.getElementById('flight-airplane').value);

    if (!flightNo || !from || !to || !departure || !arrival || !airlineId || !airplaneId) {
        showMessage('Please fill all fields', true);
        return;
    }

    // Convert HH:mm to HH:mm:ss format for TimeOnly
    const departureTime = departure.includes(':') && departure.split(':').length === 2 ? departure + ':00' : departure;
    const arrivalTime = arrival.includes(':') && arrival.split(':').length === 2 ? arrival + ':00' : arrival;

    try {
        const response = await fetch(`${API_BASE_URL}/api/flight`, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify({ 
                Flightno: flightNo, 
                From: from, 
                To: to, 
                Departure: departureTime,
                Arrival: arrivalTime,
                AirlineId: airlineId,
                AirplaneId: airplaneId
            })
        });

        if (response.ok) {
            showMessage('Flight created successfully!');
            document.getElementById('flight-no').value = '';
            document.getElementById('flight-from').value = '';
            document.getElementById('flight-to').value = '';
            document.getElementById('flight-departure').value = '';
            document.getElementById('flight-arrival').value = '';
            document.getElementById('flight-airline').value = '';
            document.getElementById('flight-airplane').value = '';
            loadFlights();
        } else {
            const error = await response.json();
            showMessage('Failed to create flight: ' + (error.message || 'Check if Airplane ID exists'), true);
        }
    } catch (error) {
        showMessage('Error: ' + error.message, true);
    }
}

// Load Airports
async function loadAirports() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/airport`);
        const airports = await response.json();
        
        const list = document.getElementById('airports-list');
        if (airports.length === 0) {
            list.innerHTML = '<p>No airports found</p>';
        } else {
            list.innerHTML = airports.map(a => `
                <div class="data-item">
                    <strong>${a.iata}</strong> (${a.icao}) - ${a.name}
                </div>
            `).join('');
        }
    } catch (error) {
        showMessage('Failed to load airports: ' + error.message, true);
    }
}

// Create Airport
async function createAirport() {
    const iata = document.getElementById('airport-iata').value;
    const icao = document.getElementById('airport-icao').value;
    const name = document.getElementById('airport-name').value;

    if (!iata || !icao || !name) {
        showMessage('Please fill all fields', true);
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/airport`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Iata: iata, Icao: icao, Name: name })
        });

        if (response.ok) {
            showMessage('Airport created successfully!');
            document.getElementById('airport-iata').value = '';
            document.getElementById('airport-icao').value = '';
            document.getElementById('airport-name').value = '';
            loadAirports();
        } else {
            const error = await response.json();
            showMessage('Failed to create airport: ' + (error.message || 'Code may already exist'), true);
        }
    } catch (error) {
        showMessage('Error: ' + error.message, true);
    }
}

// Load Airlines
async function loadAirlines() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/airline`);
        const airlines = await response.json();
        
        const list = document.getElementById('airlines-list');
        if (airlines.length === 0) {
            list.innerHTML = '<p>No airlines found</p>';
        } else {
            list.innerHTML = airlines.map(a => `
                <div class="data-item">
                    <strong>${a.iata}</strong> - ${a.airlinename}
                </div>
            `).join('');
        }
    } catch (error) {
        showMessage('Failed to load airlines: ' + error.message, true);
    }
}

// Create Airline
async function createAirline() {
    const airlineId = parseInt(document.getElementById('airline-id').value);
    const iata = document.getElementById('airline-iata').value;
    const name = document.getElementById('airline-name').value;
    const baseAirport = parseInt(document.getElementById('airline-base').value);

    if (!airlineId || !iata || !name || !baseAirport) {
        showMessage('Please fill all fields (Airline ID, IATA, Name, Base Airport)', true);
        return;
    }

    if (iata.length !== 2 && iata.length !== 3) {
        showMessage('IATA code must be 2-3 characters', true);
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/airline`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                AirlineId: airlineId,
                Iata: iata, 
                Airlinename: name,
                BaseAirport: baseAirport
            })
        });

        if (response.ok) {
            showMessage('Airline created successfully!');
            document.getElementById('airline-id').value = '';
            document.getElementById('airline-iata').value = '';
            document.getElementById('airline-name').value = '';
            document.getElementById('airline-base').value = '';
            loadAirlines();
        } else {
            const error = await response.json();
            showMessage('Failed to create airline: ' + (error.message || 'ID may already exist or base airport invalid'), true);
        }
    } catch (error) {
        showMessage('Error: ' + error.message, true);
    }
}

// Load Passengers
async function loadPassengers() {
    try {
        const response = await fetch(`${API_BASE_URL}/api/passenger`);
        const passengers = await response.json();
        
        const list = document.getElementById('passengers-list');
        if (passengers.length === 0) {
            list.innerHTML = '<p>No passengers found</p>';
        } else {
            list.innerHTML = passengers.map(p => `
                <div class="data-item">
                    <strong>${p.firstname} ${p.lastname}</strong> - Passport: ${p.passportno}
                </div>
            `).join('');
        }
    } catch (error) {
        showMessage('Failed to load passengers: ' + error.message, true);
    }
}

// Create Passenger
async function createPassenger() {
    const firstname = document.getElementById('passenger-firstname').value;
    const lastname = document.getElementById('passenger-lastname').value;
    const passport = document.getElementById('passenger-passport').value;

    if (!firstname || !lastname || !passport) {
        showMessage('Please fill all fields', true);
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/passenger`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                Firstname: firstname, 
                Lastname: lastname, 
                Passportno: passport 
            })
        });

        if (response.ok) {
            showMessage('Passenger created successfully!');
            document.getElementById('passenger-firstname').value = '';
            document.getElementById('passenger-lastname').value = '';
            document.getElementById('passenger-passport').value = '';
            loadPassengers();
        } else {
            showMessage('Failed to create passenger', true);
        }
    } catch (error) {
        showMessage('Error: ' + error.message, true);
    }
}

// Load Sensor Data
async function loadSensorData() {
    const airportCode = document.getElementById('sensor-airport').value;

    if (!airportCode) {
        showMessage('Please enter airport code', true);
        return;
    }

    try {
        const response = await fetch(`${SENSOR_API_URL}/api/sensor/${airportCode}`);
        const data = await response.json();
        
        const display = document.getElementById('sensor-data');
        display.innerHTML = `
            <div class="data-item">
                <h3>Airport: ${data.airportCode}</h3>
                <p><strong>Temperature:</strong> ${data.temperature}°C</p>
                <p><strong>Runway Occupancy:</strong> ${data.runwayOccupancy}%</p>
                <p><strong>Runway Status:</strong> ${data.runwayStatus}</p>
                <p><strong>Timestamp:</strong> ${new Date(data.timestamp).toLocaleString()}</p>
            </div>
        `;
        showMessage('Sensor data loaded!');
    } catch (error) {
        showMessage('Failed to load sensor data: ' + error.message, true);
    }
}

// Get Weather Data
async function getWeather() {
    const airportCode = document.getElementById('weather-airport').value.toUpperCase();

    if (!airportCode) {
        showMessage('Please enter airport code', true);
        return;
    }

    if (airportCode.length !== 3) {
        showMessage('Airport code must be 3 characters (IATA code)', true);
        return;
    }

    try {
        const response = await fetch(`${EXTERNAL_API_URL}/api/external/weather/${airportCode}`);
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        
        const display = document.getElementById('weather-data');
        
        // Weather emoji based on description
        let weatherEmoji = '🌤️';
        if (data.description.includes('rain')) weatherEmoji = '🌧️';
        else if (data.description.includes('cloud')) weatherEmoji = '☁️';
        else if (data.description.includes('clear')) weatherEmoji = '☀️';
        else if (data.description.includes('snow')) weatherEmoji = '❄️';
        else if (data.description.includes('storm')) weatherEmoji = '⛈️';
        
        display.innerHTML = `
            <div class="data-item" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px;">
                <h3 style="margin-top: 0;">${weatherEmoji} ${data.airportCode} - ${data.description.toUpperCase()}</h3>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-top: 15px;">
                    <div>
                        <p style="margin: 5px 0;"><strong>🌡️ Temperature:</strong> ${data.temperature.toFixed(1)}°C</p>
                        <p style="margin: 5px 0;"><strong>💧 Humidity:</strong> ${data.humidity}%</p>
                    </div>
                    <div>
                        <p style="margin: 5px 0;"><strong>💨 Wind Speed:</strong> ${data.windSpeed} m/s</p>
                        <p style="margin: 5px 0;"><strong>🕒 Updated:</strong> ${new Date(data.timestamp).toLocaleTimeString()}</p>
                    </div>
                </div>
            </div>
        `;
        showMessage('Weather data loaded successfully!');
    } catch (error) {
        document.getElementById('weather-data').innerHTML = '';
        showMessage('Failed to load weather: ' + error.message, true);
    }
}
