const cityWeatherData = JSON.parse(document.getElementById("weather-data").textContent);
const cityCoordinates = JSON.parse(document.getElementById("coords-data").textContent);
const italyData = JSON.parse(document.getElementById("italy-data").textContent);

// Weather Summary Cards
const weatherContainer = document.getElementById('weather-container');
weatherContainer.innerHTML = "";

for (const [city, weather] of Object.entries(cityWeatherData || {})) {
    let temp = weather?.main?.temp ?? 'N/A';
    let feelsLike = weather?.main?.feels_like ?? 'N/A';
    let cond = weather?.weather?.[0]?.description ?? 'N/A';
    let humidity = weather?.main?.humidity ?? 'N/A';
    let wind = weather?.wind?.speed ?? 'N/A';
    let pressure = weather?.main?.pressure ?? 'N/A';

    let cityBlock = `
         <div class="weather-detail-item">
             <span class="font-lucida">${city}</span>
             <strong class="font-lucida">${temp}°C</strong>
         </div>
         <div class="weather-detail-item"><img src="/images/humidity.png" alt="Humidity Icon">Humidity: ${humidity}%</div>
         <div class="weather-detail-item"><img src="/images/wind.png" alt="wind Icon">Wind: ${wind} m/s</div>
         <div class="weather-detail-item"><img src="/images/pressure.png" alt="pressure Icon">Pressure: ${pressure} hPa</div>
         <div class="weather-detail-item"><img src="/images/temperature-list.png" alt="feellike Icon">Feels Like: ${feelsLike}°C</div>
         <div class="weather-detail-item"><img src="/images/thunderstorm.png" alt="condition Icon">Condition: ${cond}</div>
     `;
    weatherContainer.insertAdjacentHTML('beforeend', cityBlock);
}

// Leaflet Map Initialization
document.addEventListener("DOMContentLoaded", function () {
    var map = L.map('italy-map').setView([italyData.lat, italyData.lng], 6);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    for (const [city, coords] of Object.entries(cityCoordinates || {})) {
        if (coords?.lat && coords?.lng) {
            L.marker([coords.lat, coords.lng], { title: city })
                .addTo(map)
                .bindPopup(`<b>${city}</b>`);
        }
    }
});

console.log(cityWeatherData);
