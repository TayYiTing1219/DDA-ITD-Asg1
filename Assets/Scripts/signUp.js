// Javascript code for sign-up functionality

// Sign Up
document.getElementById('signUpForm').addEventListener('submit', function (e) {
   e.preventDefault();

   // Simple form validation and submission simulation
   const formData = new FormData(this);
   const data = Object.fromEntries(formData);

   // Simulate form submission
   const signUpBtn = this.querySelector('.signUp-btn');
   const originalText = signUpBtn.textContent;
   signUpBtn.textContent = '🌱 Creating Account...'
   signUpBtn.disabled = true

   setTimeout(() => {
      alert('🌿 Account created successfully! Welcome to BirdHaven.');
      this.reset();
      signUpBtn.textContent = originalText;
      signUpBtn.disabled = false;
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

// Initialize everything
document.addEventListener('DOMContentLoaded', function () {
   console.log('DOM Content Loaded');
   updateActiveNav();
});

// Also initialize immediately in case DOM is already loaded
if (document.readyState === 'loading') {
   // DOM not ready, wait for DOMContentLoaded
} else {
   // DOM is ready
   console.log('DOM already loaded, initializing immediately');
   updateActiveNav();
}