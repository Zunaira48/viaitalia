// Currently no interactive logic was in your TSX,
// but we can add navigation active state handling

document.querySelectorAll('.nav a').forEach(link => {
  link.addEventListener('click', e => {
    document.querySelectorAll('.nav a').forEach(a => a.classList.remove('active'));
    e.target.classList.add('active');
  });
});


 document.querySelectorAll('.nav-link').forEach(link => {
      link.addEventListener('click', (e) => {
        e.preventDefault(); // Prevent default navigation for demo
        console.log(`Navigating to: ${link.getAttribute('href')}`);
        // Add actual navigation logic here (e.g., redirect or SPA routing)
      });
    });