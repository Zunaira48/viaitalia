  // Basic interactivity for navigation items
    document.querySelectorAll('.Home, .Blog, .AboutUs, .ContactUs, .PrivacyPolicy, .SignIn').forEach(item => {
      item.addEventListener('click', () => {
        console.log(`Clicked on ${item.textContent}`);
        // Add navigation logic here (e.g., redirect to respective pages)
      });
    });


document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".Frame78 a");
    const rectangle = document.querySelector(".Frame78 .Rectangle59");

    function moveRectangle(activeLink) {
        const parent = activeLink.parentElement;
        const rect = parent.getBoundingClientRect();
        const nav = document.querySelector(".Frame78");

        rectangle.style.width = rect.width + "px";
        rectangle.style.left = parent.offsetLeft + "px";
        rectangle.style.top = (parent.offsetTop + parent.offsetHeight + -10) + "px";
    }

    // Page load pe active underline set
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
        moveRectangle(navLinks[0]); // Default Home ke neeche
    }

    // Click hone par bhi move karega
    navLinks.forEach(link => {
        link.addEventListener("click", function () {
            moveRectangle(this);
        });
    });
});


     // Basic interactivity for the Privacy Policy heading
    document.querySelector('.PrivacyPolicy').addEventListener('click', () => {
      console.log('Clicked on Privacy Policy');
      // Add navigation logic here if needed (e.g., redirect to detailed privacy policy page)
    });


     // Basic interactivity for the heading
    document.querySelector('.WhatInformationWeCollect').addEventListener('click', () => {
      console.log('Clicked on What Information We Collect');
      // Add navigation or toggle logic here if needed
    });

     // Basic interactivity for the heading
    document.querySelector('.WhyWeCollectYourData').addEventListener('click', () => {
      console.log('Clicked on Why We Collect Your Data');
      // Add navigation or toggle logic here if needed
    });

      // Basic interactivity for the heading
    document.querySelector('.UseOfCookies').addEventListener('click', () => {
      console.log('Clicked on Use of Cookies');
      // Add navigation or toggle logic here if needed
    });


      // Basic interactivity for the heading
    document.querySelector('.ThirdPartyTools').addEventListener('click', () => {
      console.log('Clicked on Third-Party Tools');
      // Add navigation or toggle logic here if needed
    });


     // Basic interactivity for the heading
    document.querySelector('.HowWeProtectYourInformation').addEventListener('click', () => {
      console.log('Clicked on How We Protect Your Information');
      // Add navigation or toggle logic here if needed
    });

     // Basic interactivity for the heading
    document.querySelector('.YourChoicesRights').addEventListener('click', () => {
      console.log('Clicked on Your Choices & Rights');
      // Add navigation or toggle logic here if needed
    });

      // Basic interactivity for the slogan
    document.querySelector('.ViaitaliaPlanSmartTravelBeautifully').addEventListener('click', () => {
      console.log('Clicked on ViaItalia – Plan Smart. Travel Beautifully.');
      // Add navigation or toggle logic here if needed
    });


    // Interactivity for navigation links
    const navLinks = document.querySelectorAll('.ContactUs, .AboutUs, .TermCondition, .PrivacyPolicy, .Blog');
    navLinks.forEach(link => {
      link.addEventListener('click', () => {
        console.log(`Clicked on ${link.textContent}`);
        // Add navigation logic here (e.g., window.location.href = '/path')
      });
    });

