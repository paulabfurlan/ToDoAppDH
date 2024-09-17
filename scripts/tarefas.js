let nomeUsu = document.getElementById("nomeUsu");
let closeApp = document.getElementById("closeApp");
let statusTask = document.getElementById("statusTask");
let novaTarefa = document.getElementById("novaTarefa");
let btnCriar = document.getElementById("btnCriar");

// URL da API para pegar usuário
const apiGetMe = "https://ctd-todo-api.herokuapp.com/v1/users/getMe";

// URL para pegar as tarefas
const apiTarefas = "https://ctd-todo-api.herokuapp.com/v1/tasks";

// Pegar usuario
fetch(apiGetMe, {
  method: "GET",
  headers: {
    "Content-type": "application/json",
    Authorization: sessionStorage.getItem("jwt")
  }
})
  .then(function (resposta) {
    return resposta.json();
  })
  .then(function (data) {
    if (data.firstName)
      nomeUsu.innerText = data.firstName + " " + data.lastName;
  })
  .catch(function (erro) {
    console.log(erro);
  });

// Botao de fechar sessao
closeApp.addEventListener("click", function () {
  sessionStorage.removeItem("jwt");
  window.location.href = "index.html";
});

fetch(apiTarefas, {
  method: "GET",
  headers: {
    "Content-type": "application/json",
    Authorization: sessionStorage.getItem("jwt")
  }
})
  .then(function (resposta) {
    return resposta.json();
  })
  .then(function (data) {
    data.forEach(function (task) {
    /*let task = {
      id: 1,
      createdAt: "16/09/2024",
      description: "Tarefa de Teste",
      completed: false
    };*/

      let testIni = document.getElementsByClassName("container");
      if (testIni[0].id == "skeleton") {
        testIni[0].innerHTML = "";
        testIni[0].id = "";
      }

      const time = task.createdAt;

      let container;
      let path;
      let liTarefas = document.createElement("li");
      liTarefas.classList.add("tarefa");

      let div = document.createElement("div");
      if (task.completed) {
        container = document.getElementsByClassName("tarefas-terminadas");
        div.classList.add("done");
      } else {
        container = document.getElementsByClassName("container");
        div.classList.add("not-done");
      }
      liTarefas.appendChild(div);

      let div2 = document.createElement("div");
      let p = document.createElement("p");
      let p2 = document.createElement("p");
      div2.classList.add("descricao");
      p.classList.add("nome");
      p.innerText = task.description;
      p2.classList.add("timestamp");
      p2.innerText = "Criada em " + time.split("T")[0];
      div2.appendChild(p);
      div2.appendChild(p2);

      liTarefas.appendChild(div2);
	  
      let div3 = document.createElement("div");
      div3.classList.add("not-done");
      div3.classList.add("delete");

      liTarefas.appendChild(div3);

      container[0].appendChild(liTarefas);

      div.addEventListener("click", function () {
        if (div.classList.contains("not-done")) {
          div.classList.remove("not-done");
          div.classList.add("done");

          let tarefa = {
            description: task.description,
            completed: true
          };

          path = apiTarefas + "/" + task.id;

          fetch(path, {
            method: "PUT",
            headers: {
              "Content-type": "application/json",
              Authorization: sessionStorage.getItem("jwt")
            },
            body: JSON.stringify(tarefa)
          })
            .then(function (resposta) {
              return resposta.json();
            })
            .then(function (data) {
              if (data) document.location.reload(true);
            })
            .catch(function (erro) {
              console.log(erro);
            });
        } else {
          div.classList.remove("done");
          div.classList.add("not-done");

          let tarefa = {
            description: task.description,
            completed: false
          };

          path = apiTarefas + "/" + task.id;

          fetch(path, {
            method: "PUT",
            headers: {
              "Content-type": "application/json",
              Authorization: sessionStorage.getItem("jwt")
            },
            body: JSON.stringify(tarefa)
          })
            .then(function (resposta) {
              return resposta.json();
            })
            .then(function (data) {
              if (data) document.location.reload(true);
            })
            .catch(function (erro) {
              console.log(erro);
            });
        }
      });
	  
      // Deletar Tarefa
      div3.addEventListener("click", function () {

        path = apiTarefas + "/" + task.id;

        fetch(path, {
          method: "DELETE",
          headers: {
            "Content-type": "application/json",
            Authorization: sessionStorage.getItem("jwt"),
          }
        })
          .then(function (resposta) {
            document.location.reload(true)
            return resposta.json();
          })
          .catch(function (erro) {
            console.log(erro);
          });
      });
    });
  })
  .catch(function (erro) {
    console.log(erro);
  });

// Criar uma nova tarefa
btnCriar.addEventListener("click", function (event) {
  event.preventDefault();
  let status;

  if (statusTask.classList.contains("not-done")) {
    status = false;
  } else {
    status = true;
  }

  if (novaTarefa.value != "") {
    let tarefa = {
      description: novaTarefa.value,
      completed: status
    };

    fetch(apiTarefas, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
        Authorization: sessionStorage.getItem("jwt")
      },
      body: JSON.stringify(tarefa)
    })
      .then(function (resposta) {
        return resposta.json();
      })
      .then(function (data) {
        if (data) document.location.reload(true);
      })
      .catch(function (erro) {
        console.log(erro);
      });
  } else {
    alert("Você precisa dar um nome para a sua tarefa!");
  }
});

// Listener do check de status da nova tarefa
statusTask.addEventListener("click", function () {
  if (statusTask.classList.contains("not-done")) {
    statusTask.classList.remove("not-done");
    statusTask.classList.add("done");
  } else {
    statusTask.classList.remove("done");
    statusTask.classList.add("not-done");
  }
});
