// Import the functions you need from the SDKs you need
    import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
    import { getAnalytics } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-analytics.js";
    // TODO: Add SDKs for Firebase products that you want to use
    // https://firebase.google.com/docs/web/setup#available-libraries

    // Your web app's Firebase configuration
    // For Firebase JS SDK v7.20.0 and later, measurementId is optional
    const firebaseConfig = {
        apiKey: "AIzaSyC6kq78oZEICPDUYMfYnI3BaPdVSP4aeBY",
        authDomain: "birdhaven-a93f3.firebaseapp.com",
        projectId: "birdhaven-a93f3",
        storageBucket: "birdhaven-a93f3.firebasestorage.app",
        messagingSenderId: "1018492623542",
        appId: "1:1018492623542:web:6522efc63239ace606e417",
        measurementId: "G-J4HM5E61Q8"
    };

    // Initialize Firebase
    const app = initializeApp(firebaseConfig);
    const analytics = getAnalytics(app);

import { getDatabase, ref, child, get } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-database.js";

const dbRef = ref(getDatabase());
const usersRef = child(dbRef, 'users');

function storeUserData(userId, name, email, password) {
    const db = getDatabase();
    const userRef = ref(db, 'users/' + userId); 
    Set(userRef, {
        username: name,
        email: email,
        password: password
    });
}



addEventListener('DOMContentLoaded', () => {
    get(usersRef).then((snapshot) => {
        if (snapshot.exists()) {
            console.log(snapshot.val());
        } else {
            console.log("No data available");
        }
    }).catch((error) => {
        console.error(error);
    }
    );
});

document.getElementById("signUp-btn").addEventListener('submit', function (e) {
   e.preventDefault();
    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const userId = Date.now().toString(); // Simple unique ID based on timestamp

    storeUserData(userId, name, email, password);
});
