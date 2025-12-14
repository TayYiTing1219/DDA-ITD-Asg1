// Javascript code for sign-up functionality

// Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    import { getDatabase, ref, set } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";
   import { getAuth, createUserWithEmailAndPassword } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";

  // TODO: Add SDKs for Firebase products that you want to use
  // https://firebase.google.com/docs/web/setup#available-libraries

  // Your web app's Firebase configuration
  // For Firebase JS SDK v7.20.0 and later, measurementId is optional
  const firebaseConfig = {
    apiKey: "AIzaSyC6kq78oZEICPDUYMfYnI3BaPdVSP4aeBY",
    authDomain: "birdhaven-a93f3.firebaseapp.com",
    databaseURL: "https://birdhaven-a93f3-default-rtdb.asia-southeast1.firebasedatabase.app",
    projectId: "birdhaven-a93f3",
    storageBucket: "birdhaven-a93f3.firebasestorage.app",
    messagingSenderId: "1018492623542",
    appId: "1:1018492623542:web:6522efc63239ace606e417",
    measurementId: "G-J4HM5E61Q8"
  };

  // Initialize Firebase
    const app = initializeApp(firebaseConfig);
    const db = getDatabase(app);
   const auth = getAuth(app);

   // Sign Up
    // Create user data in the database
    function createUserData() {
        const name = document.getElementById('name').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        createUserWithEmailAndPassword(auth, email, password).then((userCredential) => {
            const user = userCredential.user;
            const newUser = {
            name: name,
            email: email,
            password: password,
            birdsSeen: {
                    kingfisher: 0,
                    mallardDuck: 0,
                    peacock: 0,
                    }
        };
        set(ref(db, `users/${user.uid}`), newUser).then(() => {
            console.log("Player data created successfully.");

            // Simulate form submission
            const signUpBtn = document.getElementById("signUp-btn");
            const originalText = signUpBtn.textContent;

            signUpBtn.textContent = '🌱 Creating Account...'
            signUpBtn.disabled = true

            setTimeout(() => {
                alert(`🌿 Account created successfully! Welcome to BirdHaven ${userData.name}!`);
                this.reset();
                signUpBtn.textContent = originalText;
                signUpBtn.disabled = false;
            }, 1500);

            setTimeout(() => {
                    window.location.href = "index.html";
                }, 1600);
        }).catch((error) => {
            console.error("Error creating player data: ", error);
        });
    })
   };

    document.getElementById('signUp-btn').addEventListener('click', function (e) {
    e.preventDefault();
    createUserData();
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