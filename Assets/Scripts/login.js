// JavaScript code for login functionality

// Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    import { firebaseConfig } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-config.js";
    import { getDatabase, ref, get } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";
    import { getAuth, signInWithEmailAndPassword } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";

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

   // Authentication
        function login() {
         const password = document.getElementById("password").value;
         const email = document.getElementById("email").value;

         signInWithEmailAndPassword(auth, email, password)
         .then((userCredential) => {
            if (userCredential.user) {
               const user = userCredential.user;
               
               get(ref(db, 'users/' + user.uid)).then((snapshot) => {
                     if (snapshot.exists()) {
                     const userData = snapshot.val();
                     const originalText = loginButton.textContent;
                     loginButton.textContent = '🌱 Logging In...'
                     loginButton.disabled = true

                     // Load the player's profile
                     setTimeout(() => {
                        alert(`🌿 Logged in successfully! Welcome back to BirdHaven ${userData.name}!`);
                        this.reset();
                        loginButton.textContent = originalText;
                        loginButton.disabled = false;
                        }, 1500);

                     setTimeout(() => {
                        window.location.href = "index.html";
                        }, 1600);
                     }
                     });
            };
         }) 
         .catch((error) => {
            if (error.code == "auth/invalid-email") {
               alert("Invalid email. Please try again.");
            } else if (error.code == "auth/wrong-password") {
               alert("Incorrect password. Please try again.");
            } else if (error.code == "auth/user-not-found") {
               alert("No account found with this email. Please sign up first.");
            } else {
               alert(`Unable to sign in: ${error.code}`);
            }
         });
        }

        document.getElementById("loginButton").addEventListener("click", (e) => {
            e.preventDefault();
            login();
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