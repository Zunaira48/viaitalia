// Currently no interactive logic was in your TSX,
// but we can add navigation active state handling

document.querySelectorAll('.nav a').forEach(link => {
  link.addEventListener('click', e => {
    document.querySelectorAll('.nav a').forEach(a => a.classList.remove('active'));
    e.target.classList.add('active');
  });
});

document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".nav.section a");
    const rectangle = document.querySelector(".nav.section .Rectangle59");

    function moveRectangle(activeLink) {
        const parent = activeLink.parentElement;
        rectangle.style.width = parent.offsetWidth + "px";
        rectangle.style.left = parent.offsetLeft + "px";
        rectangle.style.top = (parent.offsetTop + parent.offsetHeight - 5) + "px";
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


 document.querySelectorAll('.nav-link').forEach(link => {
      link.addEventListener('click', (e) => {
        e.preventDefault(); // Prevent default navigation for demo
        console.log(`Navigating to: ${link.getAttribute('href')}`);
        // Add actual navigation logic here (e.g., redirect or SPA routing)
      });
    });