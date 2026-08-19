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
    const navLinks = document.querySelectorAll(".nav a");
    const rectangle = document.querySelector(".Rectangle59");

    function moveRectangle(activeLink) {
        const parent = activeLink.parentElement;
        const rect = parent.getBoundingClientRect();
        const nav = document.querySelector(".nav");

        rectangle.style.width = rect.width + "px";
        rectangle.style.left = parent.offsetLeft + "px";
        rectangle.style.top = (parent.offsetTop + parent.offsetHeight + -10) + "px";
    }

    // Default load → jis page pe ho uska active underline
    const currentPath = window.location.pathname.toLowerCase();
    let matched = false;

    navLinks.forEach(link => {
        const href = link.getAttribute("href")?.toLowerCase();
        if (currentPath.includes(href)) {
            moveRectangle(link);
            matched = true;
        }
    });

    // agar koi match nahi mila, default Home ke neeche dikha do
    if (!matched && navLinks.length > 0) {
        moveRectangle(navLinks[0]);
    }

    // On click move bhi karega
    navLinks.forEach(link => {
        link.addEventListener("click", function () {
            moveRectangle(this);
        });
    });
});




 



// footer nav 

// Interactivity for navigation links
    const navLinks = document.querySelectorAll('.ContactUs, .AboutUs, .TermCondition, .PrivacyPolicy, .Blog');
    navLinks.forEach(link => {
      link.addEventListener('click', () => {
        console.log(`Clicked on ${link.textContent}`);
        // Add navigation logic here (e.g., window.location.href = '/path')
      });
    });