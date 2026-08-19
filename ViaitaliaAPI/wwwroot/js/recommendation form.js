
// Utility: Single Select Buttons
    function makeSingleSelect(containerSelector, buttonSelector, activeClass = 'active', inputId) {
      const container = document.querySelector(containerSelector);
      if (!container) return;
      
      const buttons = container.querySelectorAll(buttonSelector);
      const input = document.getElementById(inputId);
      
      buttons.forEach(button => {
        button.addEventListener('click', () => {
          buttons.forEach(btn => btn.classList.remove(activeClass));
          button.classList.add(activeClass);
          input.value = button.getAttribute('data-value');
          clearError(inputId);
        });
      });
    }
    
    // Clear validation error
    function clearError(inputId) {
      const errorElement = document.getElementById(inputId + '-error');
      if (errorElement) {
        errorElement.textContent = '';
      }
    }
    
    // Show validation error
    function showError(inputId, message) {
      const errorElement = document.getElementById(inputId + '-error');
      if (errorElement) {
        errorElement.textContent = message;
      }
    }
    
    // Validate form
    function validateForm() {
      let isValid = true;
      
      // Check GroupType
      if (!document.getElementById('GroupType').value) {
        showError('GroupType', 'Please select a group type');
        isValid = false;
      }
      
      // Check TripDuration
      if (!document.getElementById('TripDuration').value) {
        showError('TripDuration', 'Please enter trip duration');
        isValid = false;
      }
      
      // Check RequiresWheelchair
      if (!document.getElementById('RequiresWheelchair').value) {
        showError('RequiresWheelchair', 'Please select wheelchair requirement');
        isValid = false;
      }
      
      // Check SelectedTags
      if (!document.getElementById('SelectedTags').value) {
        showError('SelectedTags', 'Please select at least one theme');
        isValid = false;
      }
      
      // Check SelectedAirport
      if (!document.getElementById('SelectedAirport').value) {
        showError('SelectedAirport', 'Please select an airport');
        isValid = false;
      }
      
      // Check BudgetType
      if (!document.getElementById('BudgetType').value) {
        showError('BudgetType', 'Please select a budget type');
        isValid = false;
      }
      
      // Check HotelStarRating
      if (!document.getElementById('HotelStarRating').value) {
        showError('HotelStarRating', 'Please select hotel star rating');
        isValid = false;
      }
      
      return isValid;
    }
    
    // 1. Companions (Friend / Couple / Family)
    makeSingleSelect('#companions', '.choice-card', 'active', 'GroupType');
    
    // 2. Wheelchair Accessibility (Yes / No)
    makeSingleSelect('#accessibility', '.radio-btn', 'active', 'RequiresWheelchair');
    
    // 3. Budget (Low / Medium / High)
    makeSingleSelect('#budget', '.budget-card', 'active', 'BudgetType');
    
    // 4. Theme (Ancient / Nature / etc.)
    document.querySelectorAll('#theme .select-text p').forEach(p => {
      p.addEventListener('click', () => {
        p.classList.toggle('selected');
        
        // Update hidden input with comma-separated values
        const selectedTags = Array.from(document.querySelectorAll('#theme .select-text p.selected'))
          .map(tag => tag.getAttribute('data-value'))
          .join(',');
        
        document.getElementById('SelectedTags').value = selectedTags;
        clearError('SelectedTags');
      });
    });
    
    // 5. Hotel Rating (1 to 5 star)
    makeSingleSelect('#activities', '.activity-card', 'active', 'HotelStarRating');
    
    // 6. Duration Counter
    const plusBtn = document.querySelector('#duration .counter-btn:last-child');
    const minusBtn = document.querySelector('#duration .counter-btn:first-child');
    const daysInput = document.getElementById('days-input');
    const tripDurationInput = document.getElementById('TripDuration');
    
    // Initialize trip duration
    tripDurationInput.value = daysInput.value;
    
    plusBtn.addEventListener('click', () => {
      let value = parseInt(daysInput.value);
      if (!isNaN(value)) {
        daysInput.value = value + 1;
        tripDurationInput.value = daysInput.value;
        clearError('TripDuration');
      }
    });
    
    minusBtn.addEventListener('click', () => {
      let value = parseInt(daysInput.value);
      if (!isNaN(value) && value > 1) {
        daysInput.value = value - 1;
        tripDurationInput.value = daysInput.value;
        clearError('TripDuration');
      }
    });
    
    // Airport selection
    document.getElementById('airportSelect').addEventListener('change', function() {
      document.getElementById('SelectedAirport').value = this.value;
      clearError('SelectedAirport');
    });
    
    // Form submission
    document.getElementById('tripForm').addEventListener('submit', function (e) {
      e.preventDefault();
      
      if (validateForm()) {
        // In a real application, you would submit the form to the server
        // For demo purposes, we'll just show an alert
        alert('Form submitted successfully! Travel plan will be generated based on your preferences.');
        
        // Log form data
        console.log('Form Data:', {
          GroupType: document.getElementById('GroupType').value,
          TripDuration: document.getElementById('TripDuration').value,
          RequiresWheelchair: document.getElementById('RequiresWheelchair').value,
          SelectedTags: document.getElementById('SelectedTags').value,
          SelectedAirport: document.getElementById('SelectedAirport').value,
          BudgetType: document.getElementById('BudgetType').value,
          HotelStarRating: document.getElementById('HotelStarRating').value
        });
      }
    });
  // 4. Theme (Selected Tags)
    document.querySelectorAll('#theme .select-text p').forEach(p => {
      p.addEventListener('click', () => {
        p.classList.toggle('selected');
        
        // Update hidden input with comma-separated values
        const selectedTags = Array.from(document.querySelectorAll('#theme .select-text p.selected'))
          .map(tag => tag.getAttribute('data-value'))
          .join(',');
        
        document.getElementById('SelectedTags').value = selectedTags;
        
        // Clear validation error when selection is made
        if (selectedTags) {
          document.querySelector('[data-valmsg-for="SelectedTags"]').textContent = '';
        }
      });
    });



// Script for plus/minus buttons
document.addEventListener('DOMContentLoaded', function () {
    // Get elements by ID for more reliable selection
    const decreaseBtn = document.getElementById('decreaseBtn');
    const increaseBtn = document.getElementById('increaseBtn');
    const durationInput = document.getElementById('durationInput');

    // Check if all elements exist before adding event listeners
    if (decreaseBtn && increaseBtn && durationInput) {
        decreaseBtn.addEventListener('click', function () {
            let currentValue = parseInt(durationInput.value) || 0;
            if (currentValue > 1) {
                durationInput.value = currentValue - 1;
            }
        });

        increaseBtn.addEventListener('click', function () {
            let currentValue = parseInt(durationInput.value) || 0;
            durationInput.value = currentValue + 1;
        });
    } else {
        console.error('One or more elements not found:', {
            decreaseBtn: decreaseBtn,
            increaseBtn: increaseBtn,
            durationInput: durationInput
        });
    }
});