// Elements
let txtNome = document.getElementById("txtNome");
let txtSobrenome = document.getElementById("txtSobrenome");
let txtEmail = document.getElementById("txtEmail");
let txtSenha = document.getElementById("txtSenha");
let txtRepSenha = document.getElementById("txtRepSenha");
let btnCriar = document.getElementById("btnCriar");
let erroNome = document.getElementById("erroNome");
let erroSobrenome = document.getElementById("erroSobrenome");
let erroEmail = document.getElementById("erroEmail");
let erroSenha = document.getElementById("erroSenha");
let erroRepSenha = document.getElementById("erroRepSenha");
let loader = document.getElementById("loader");
let body = document.getElementsByTagName("body")[0];

let erroCampos = [true, true, true, true, true];

// API URLs to register the new user
const apiCadastro = "https://app-todoapp-southbr-dev-001-dxfbhwbufagvdcez.brazilsouth-01.azurewebsites.net/api/v1/Auth/Register";
//const apiCadastro = "https://localhost:7042/api/v1/Auth/Register";
const apiUsuario = "https://app-todoapp-southbr-dev-001-dxfbhwbufagvdcez.brazilsouth-01.azurewebsites.net/api/v1/Users";
//const apiCadastro = "https://localhost:7042/api/v1/Users";

txtNome.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtNome.value === "") {
    if (!txtNome.classList.contains("erro")) txtNome.classList.add("erro");
    erroNome.innerText = "Mandatory field!!";
  } else if (txtNome.value.length === 1) {
    if (!txtNome.classList.contains("erro")) txtNome.classList.add("erro");
    erroNome.innerText = "The name must have at least two letters.";
  } else {
    txtNome.classList.remove("erro");
    erroNome.innerText = "";
    erros = false;
  }

  erroCampos[0] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux == erroCampos.length) btnCriar.disabled = false;
  else btnCriar.disabled = true;
});

txtSobrenome.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtSobrenome.value == "") {
    if (!txtSobrenome.classList.contains("erro"))
      txtSobrenome.classList.add("erro");
    erroSobrenome.innerText = "Mandatory field!!";
  } else if (txtSobrenome.value.length == 1) {
    if (!txtSobrenome.classList.contains("erro"))
      txtSobrenome.classList.add("erro");
    erroSobrenome.innerText = "The last name must have at least two letters.";
  } else {
    txtSobrenome.classList.remove("erro");
    erroSobrenome.innerText = "";
    erros = false;
  }

  erroCampos[1] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux == erroCampos.length) btnCriar.disabled = false;
  else btnCriar.disabled = true;
});

txtEmail.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtEmail.value == "") {
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

  erroCampos[2] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux == erroCampos.length) btnCriar.disabled = false;
  else btnCriar.disabled = true;
});

txtSenha.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtSenha.value == "") {
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

  erroCampos[3] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux == erroCampos.length) btnCriar.disabled = false;
  else btnCriar.disabled = true;
});

txtRepSenha.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtRepSenha.value == "") {
    if (!txtRepSenha.classList.contains("erro"))
      txtRepSenha.classList.add("erro");
    erroRepSenha.innerText = "Mandatory field!!";
  } else if (txtRepSenha.value != txtSenha.value) {
    if (!txtRepSenha.classList.contains("erro"))
      txtRepSenha.classList.add("erro");
    erroRepSenha.innerText = "The password here needs to be the same as above.";
  } else {
    txtRepSenha.classList.remove("erro");
    erroRepSenha.innerText = "";
    erros = false;
  }

  erroCampos[4] = erros;

  erroCampos.forEach(function (erro) {
    if (!erro) aux++;
  });
  if (aux == erroCampos.length) btnCriar.disabled = false;
  else btnCriar.disabled = true;
});

btnCriar.addEventListener("click", function (event) {
  event.preventDefault();

  if (!btnCriar.disabled) {
    loader.style.visibility = "visible";
    body.style.opacity = "0.5";

    let cadastro = {
      username: txtEmail.value,
      password: txtSenha.value,
      roles: ["User", "Adm"]
    };
    let usuario = {
      name: txtNome.value,
      lastName: txtSobrenome.value,
      email: txtEmail.value
    };

    fetch(apiCadastro, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
        'Access-Control-Allow-Origin': '*'
      },
      body: JSON.stringify(cadastro)
    })
    .then(function (resposta) {
      if(resposta.ok)
      {
        fetch(apiUsuario, {
          method: "POST",
          headers: {
            "Content-type": "application/json",
            'Access-Control-Allow-Origin': '*'
          },
          body: JSON.stringify(usuario)
        })
        .then(function (resposta2) {
          if(resposta2.ok)
            window.location.href = "index.html";
          else{
            alert("Error while creating the new User!");
            loader.style.visibility = "hidden";
            body.style.opacity = "1";
          }
        })
        .catch(function (erro) {
          console.log(erro);
          alert("Error while creating the new User!");
          loader.style.visibility = "hidden";
          body.style.opacity = "1";
        });
      }
      else{
        alert("Error while creating the new User!");
        loader.style.visibility = "hidden";
        body.style.opacity = "1";
      }
    })
    .catch(function (erro) {
      console.log(erro);
      alert("Error while creating the new User!");
      loader.style.visibility = "hidden";
      body.style.opacity = "1";
    });
  }
});
