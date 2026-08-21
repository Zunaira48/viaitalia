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
        <div class="weather-city-card">
            <div class="weather-city-card__header">
                <span>${city}</span>
                <strong class="temp">${temp}°C</strong>
            </div>
            <div class="weather-stat"><img src="/images/humidity.png" alt="Humidity icon">Humidity: ${humidity}%</div>
            <div class="weather-stat"><img src="/images/wind.png" alt="Wind icon">Wind: ${wind} m/s</div>
            <div class="weather-stat"><img src="/images/pressure.png" alt="Pressure icon">Pressure: ${pressure} hPa</div>
            <div class="weather-stat"><img src="/images/temperature-list.png" alt="Feels like icon">Feels Like: ${feelsLike}°C</div>
            <div class="weather-stat"><img src="/images/thunderstorm.png" alt="Condition icon">Condition: ${cond}</div>
        </div>
    `;
    weatherContainer.insertAdjacentHTML('beforeend', cityBlock);
}

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