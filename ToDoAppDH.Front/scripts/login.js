let txtEmail = document.getElementById("txtEmail");
let txtSenha = document.getElementById("txtSenha");
let btnAcessar = document.getElementById("btnAcessar");
let erroEmail = document.getElementById("erroEmail");
let erroSenha = document.getElementById("erroSenha");

let erroCampos = [true, true];

// API Login URLs
const apiLogin = "https://app-todoapp-southbr-dev-001-dxfbhwbufagvdcez.brazilsouth-01.azurewebsites.net/api/v1/Auth/Login";
//const apiLogin = "https://localhost:7042/api/v1/Auth/Login";

txtEmail.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtEmail.value === "") {
    if (!txtEmail.classList.contains("erro")) txtEmail.classList.add("erro");
    erroEmail.innerText = "Mandatory field!!";
  } else if (!(txtEmail.value.includes("@") && txtEmail.value.includes("."))) {
    if (!txtEmail.classList.contains("erro")) txtEmail.classList.add("erro");
    erroEmail.innerText = "The e-mail address needs to be valid.";
  } else {
    txtEmail.classList.remove("erro");
    erroEmail.innerText = "";
    erros = false;
  }

  erroCampos[0] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux === erroCampos.length) btnAcessar.disabled = false;
  else btnAcessar.disabled = true;
});

txtSenha.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtSenha.value === "") {
    if (!txtSenha.classList.contains("erro")) txtSenha.classList.add("erro");
    erroSenha.innerText = "Mandatory field!!";
  } else if (txtSenha.value.length < 6) {
    if (!txtSenha.classList.contains("erro")) txtSenha.classList.add("erro");
    erroSenha.innerText = "The password must contain a minimum of 6 characters.";
  } else {
    txtSenha.classList.remove("erro");
    erroSenha.innerText = "";
    erros = false;
  }

  erroCampos[1] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux === erroCampos.length) btnAcessar.disabled = false;
  else btnAcessar.disabled = true;
});

btnAcessar.addEventListener("click", function (event) {
  event.preventDefault();

  if (!btnAcessar.disabled) {
    let login = {
      username: txtEmail.value,
      password: txtSenha.value,
      expiration: 24
    };

    fetch(apiLogin, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
        'Access-Control-Allow-Origin': '*'
      },
      body: JSON.stringify(login)
    })
      .then(function (resposta) {
        return resposta.json();
      })
      .then(function (data) {
        if (data.jwtToken) {
          sessionStorage.setItem("jwt", data.jwtToken);
          console.log(sessionStorage.getItem("jwt"));
          sessionStorage.setItem("email", txtEmail.value);
          window.location.href = "tarefas.html";
        } else {
          alert("Username and / or password is incorrect!");
        }
      })
      .catch(function (erro) {
        console.log(erro);
      });
  }
});
