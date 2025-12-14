using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine.UI;
using System;
using Firebase.Auth;

public class DatabaseController : MonoBehaviour
{

    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField username;
    [SerializeField] Button loginButton;

    user myUser;
   
   [SerializeField] public GameObject signUp;

   [SerializeField] GameObject signUpUI;
      

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


      public void CreatePlayer() // Create new player
    {
      var db = FirebaseDatabase.DefaultInstance.RootReference;

      user user = new user();
      user.name = username.text;
      user.email = email.text;
      user.password = password.text;
      var json = JsonUtility.ToJson(user, true); // pretty print

      db.Child("users").Child(user.name).SetRawJsonValueAsync(json);
      Debug.Log("Your Username: " + user.name);
    }

    public void hideUI()
   {
      signUpUI.SetActive(false);
   }
}
