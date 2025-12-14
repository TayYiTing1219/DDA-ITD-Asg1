// JavaScript code for app functionality

// Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    import { firebaseConfig } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-config.js";
    import { getDatabase, ref, get } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";
    import { getAuth } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";

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

   function storeUserData(userId, userData) {
       return set(ref(db, 'users/' + userId), userData);
   }
   
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