let txtEmail = document.getElementById("txtEmail");
let txtSenha = document.getElementById("txtSenha");
let btnAcessar = document.getElementById("btnAcessar");
let erroEmail = document.getElementById("erroEmail");
let erroSenha = document.getElementById("erroSenha");

let erroCampos = [true, true];

// URL da API para Logar
const apiLogin = "https://ctd-todo-api.herokuapp.com/v1/users/login";

txtEmail.addEventListener("keyup", function () {
  let erros = true;
  let aux = 0;

  if (txtEmail.value === "") {
    if (!txtEmail.classList.contains("erro")) txtEmail.classList.add("erro");
    erroEmail.innerText = "Campo obrigatório!!";
  } else if (!(txtEmail.value.includes("@") && txtEmail.value.includes("."))) {
    if (!txtEmail.classList.contains("erro")) txtEmail.classList.add("erro");
    erroEmail.innerText = "O e-mail precisa ser válido";
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
    erroSenha.innerText = "Campo obrigatório!!";
  } else if (txtSenha.value.length < 6) {
    if (!txtSenha.classList.contains("erro")) txtSenha.classList.add("erro");
    erroSenha.innerText = "Mínimo de 6 caracteres.";
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
      email: txtEmail.value,
      password: txtSenha.value
    };

    fetch(apiLogin, {
      method: "POST",
      headers: {
        "Content-type": "application/json"
      },
      body: JSON.stringify(login)
    })
      .then(function (resposta) {
        return resposta.json();
      })
      .then(function (data) {
        console.log(data);
        if (data.jwt) {
          sessionStorage.setItem("jwt", data.jwt);
          window.location.href = "tarefas.html";
        } else {
          alert("Usuário ou senha incorretos!");
        }
      })
      .catch(function (erro) {
        console.log(erro);
      });
  }
});
