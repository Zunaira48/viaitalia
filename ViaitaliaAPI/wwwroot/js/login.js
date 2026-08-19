// Redirect to Sign In or Sign Up
document.getElementById('redirectSignInBtn').addEventListener('click', function () {
  alert('Redirecting to Sign Up page...');
  // Example redirect: window.location.href = 'signup.html';
});

// Forgot Password
document.getElementById('forgotPasswordBtn').addEventListener('click', function () {
  alert('Forgot password clicked!');
});

// Actual Login Logic
document.getElementById('loginBtn').addEventListener('click', function () {
  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value.trim();

  if (!email || !password) {
    alert('Please fill all fields');
  } else {
    alert(`Login Successful!\nEmail: ${email}`);
    // Add further login logic here
  }
});