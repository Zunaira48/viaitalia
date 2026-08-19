document.addEventListener('DOMContentLoaded', () => {
  const navItems = document.querySelectorAll('.Home, .Blog, .AboutUs, .ContactUs, .PrivacyPolicy, .SignIn');
  
  navItems.forEach(item => {
    item.addEventListener('click', () => {
      console.log(`${item.textContent} clicked`);
      // Add navigation logic here (e.g., redirect to pages or toggle active states)
    });
  });
});


document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".Frame81 a");
    const rectangle = document.querySelector(".Frame81 .Rectangle59");

    function moveRectangle(activeLink) {
        const parent = activeLink.parentElement;
        rectangle.style.width = parent.offsetWidth + "px";
        rectangle.style.left = parent.offsetLeft + "px";
        rectangle.style.top = (parent.offsetTop + parent.offsetHeight - 5) + "px";
    }

    // Page load → active page detect kare
    const currentPath = window.location.pathname.toLowerCase();
    let matched = false;

    navLinks.forEach(link => {
        const href = link.getAttribute("href")?.toLowerCase();
        if (currentPath.includes(href)) {
            moveRectangle(link);
            matched = true;
        }
    });

    if (!matched && navLinks.length > 0) {
        moveRectangle(navLinks[0]); // default Home
    }

    // Click hone par bhi move karega
    navLinks.forEach(link => {
        link.addEventListener("click", function () {
            moveRectangle(this);
        });
    });
});






   // No dynamic JavaScript functionality is required based on the provided code
        // Placeholder for any future interactivity
        document.addEventListener('DOMContentLoaded', () => {
            console.log('Terms and Conditions page loaded');
        });
