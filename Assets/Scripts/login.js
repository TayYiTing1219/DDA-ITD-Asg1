// JavaScript code for login functionality

// Login
document.getElementById('loginForm').addEventListener('submit', function (e) {
   e.preventDefault();

   // Simple form validation and submission simulation
   const formData = new FormData(this);
   const data = Object.fromEntries(formData);

   // Simulate form submission
   const loginBtn = this.querySelector('.submit-btn');
   const originalText = loginBtn.textContent;
   loginBtn.textContent = '🌱 Logging In...'
   loginBtn.disabled = true

   setTimeout(() => {
      alert('🌿 Logged in successfully! Welcome back to BirdHaven.');
      this.reset();
      loginBtn.textContent = originalText;
      loginBtn.disabled = false;
   }, 1500);
   
   setTimeout(() => {
        window.location.href = "index.html";
    }, 1600);
});

// Navbar scroll effect and active link management
const navbar = document.getElementById('navbar');
const sections = document.querySelectorAll('section');
const navLinks = document.querySelectorAll('.nav-link');

function updateActiveNav() {
   const scrollPosition = window.pageYOffset + 100;

   sections.forEach((section, index) => {
      const sectionTop = section.offsetTop;
      const sectionHeight = section.offsetHeight;

      if (scrollPosition >= sectionTop && scrollPosition < sectionTop + sectionHeight) {
         navLinks.forEach(link => link.classList.remove('active'));
         const currentNav = document.querySelector(`.nav-link[href="#${section.id}"]`);
         if (currentNav) currentNav.classList.add('active');
      }
   });
}

window.addEventListener('scroll', () => {
   // Navbar scroll effect
   if (window.scrollY > 50) {
      navbar.classList.add('scrolled');
   } else {
      navbar.classList.remove('scrolled');
   }

   // Update active navigation
   updateActiveNav();
});

// Smooth scrolling
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
   anchor.addEventListener('click', function (e) {
      e.preventDefault();
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
         target.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
         });
      }
   });
});