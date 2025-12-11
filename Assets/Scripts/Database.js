// Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    import { getAnalytics } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-analytics.js";
    import { getDatabase, child, ref, set } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";

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
    // const users = child(db, 'users');
    

    // addEventListener('DOMContentLoaded', () => {
    //     get(usersRef).then((snapshot) => {
    //         if (snapshot.exists()) {
    //             console.log(snapshot.val());
    //         } else {
    //             console.log("No data available");
    //         }
    //     }).catch((error) => {
    //         console.error(error);
    //     }
    //     );
    // });

    // Authentication
        // import { getAuth, loginWithNameAndPassword } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";
        // const auth = getAuth(app);

        // document.getElementById("loginButton").addEventListener("click", (e) => {
        //     e.preventDefault();
        //     login();
        // });

        // function login() {
        //     const password = document.getElementById("password").value;
        //     const name = document.getElementById("name").value;
        //     const db = getDatabase(app);
        //     const userId = child(db, users, name);

        //     loginWithNameAndPassword(auth, name, password, userId)
        //     .then((userCredential) => {
        //         const user = userCredential.user;
        //         alert(`${user.name} is logged in with userID ${user.userId}`);

        //         // Load the player's profile
        //         const playerRef = child(users, user.name);
        //         get(playerRef).then((snapshot) => {
        //         const playerData = snapshot.val();
        //         alert(`${user.name} has a score of ${playerData.score}!`);
        //         });
        //     })
        //     .catch((error) => {
        //         alert(`${error.code}: ${error.message}`);
        //     });
        // }

    // Sign Up
    // Create user data in the database
    function createUserData() {
        const name = document.getElementById('name').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const user = ref(db, "users");
        const newUser = child(user, name);

        set(newUser, {
                username: name,
                email: email,
                password: password,
                birdsSeen: {
                    kingfisher: 0,
                    mallardDuck: 0,
                    peacock: 0,
                    }
                },
        ).then(() => {
            console.log("Player data created successfully.");

            // Simulate form submission
            const signUpBtn = document.getElementById("signUp-btn");
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
        }).catch((error) => {
            console.error("Error creating player data: ", error);
        });
    }

    document.getElementById('signUp-btn').addEventListener('click', function (e) {
    e.preventDefault();
    createUserData();
    });