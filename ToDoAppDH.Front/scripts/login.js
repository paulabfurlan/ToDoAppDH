let txtEmail = document.getElementById("txtEmail");
let txtSenha = document.getElementById("txtSenha");
let btnAcessar = document.getElementById("btnAcessar");
let erroEmail = document.getElementById("erroEmail");
let erroSenha = document.getElementById("erroSenha");
let loader = document.getElementById("loader");
let body = document.getElementsByTagName("body")[0];

// API Login URLs
const apiLogin = "https://app-todoapp-southbr-dev-002-buf5ape8h4eqc6fh.brazilsouth-01.azurewebsites.net/api/v1/Auth/Login";
//const apiLogin = "https://localhost:7042/api/v1/Auth/Login";

txtEmail.value = "";
txtSenha.value = "";

function testaEmail() {
  if (txtEmail.value == "") {
    if (!txtEmail.classList.contains("erro")) 
      txtEmail.classList.add("erro");
    erroEmail.innerText = "Mandatory field!!";
  } else if (!(txtEmail.value.includes("@") && txtEmail.value.includes("."))) {
    if (!txtEmail.classList.contains("erro")) 
      txtEmail.classList.add("erro");
    erroEmail.innerText = "The e-mail address needs to be valid.";
  } else {
    txtEmail.classList.remove("erro");
    erroEmail.innerText = "";
    return true;
  }

  return false;
}

function testaSenha() {
  if (txtSenha.value == "") {
    if (!txtSenha.classList.contains("erro")) txtSenha.classList.add("erro");
    erroSenha.innerText = "Mandatory field!!";
  } else if (txtSenha.value.length < 6) {
    if (!txtSenha.classList.contains("erro")) txtSenha.classList.add("erro");
    erroSenha.innerText = "The password must contain a minimum of 6 characters.";
  } else {
    txtSenha.classList.remove("erro");
    erroSenha.innerText = "";
    return true;
  }

  return false;
}

btnAcessar.addEventListener("click", function (event) {
  event.preventDefault();
  let teste1 = testaEmail();
  let teste2 = testaSenha();

  if(teste1 && teste2)
  {
    loader.style.visibility = "visible";
    body.style.opacity = "0.5";

    let login = {
      username: txtEmail.value.toLowerCase(),
      password: txtSenha.value,
      expiration: 24
    };

    fetch(apiLogin, {
      method: "POST",
      headers: {
        "Content-type": "application/json"
      },
      body: JSON.stringify(login),
      mode: "cors"
    })
      .then(function (resposta) {
        return resposta.json();
      })
      .then(function (data) {
        if (data.jwtToken) {
          sessionStorage.setItem("jwt", data.jwtToken);
          console.log(sessionStorage.getItem("jwt"));
          sessionStorage.setItem("email", txtEmail.value.toLowerCase());
          window.location.href = "tarefas.html";
        } else {
          alert("Username and / or password is incorrect!");
          loader.style.visibility = "hidden";
          body.style.opacity = "1";
        }
      })
      .catch(function (erro) {
        loader.style.visibility = "hidden";
        body.style.opacity = "1";
        console.log(erro);
        alert("Username and / or password is incorrect!");
      });
  }
});
