let nomeUsu = document.getElementById("nomeUsu");
let closeApp = document.getElementById("closeApp");
let statusTask = document.getElementById("statusTask");
let novaTarefa = document.getElementById("novaTarefa");
let btnCriar = document.getElementById("btnCriar");
let spanChar = document.getElementById("numCaracteres");
let loader = document.getElementById("loader");
let body = document.getElementsByTagName("body")[0];

// API URL to get user
const apiGetMe = "https://app-todoapp-southbr-dev-001-dxfbhwbufagvdcez.brazilsouth-01.azurewebsites.net/api/v1/Users";
//const apiGetMe = "https://localhost:7042/api/v1/Users";

// API URL to get the tasks
const apiTarefas = "https://app-todoapp-southbr-dev-001-dxfbhwbufagvdcez.brazilsouth-01.azurewebsites.net/api/v1/Tasks";
//const apiTarefas = "https://localhost:7042/api/v1/Tasks";

loader.style.visibility = "visible";
body.style.opacity = "0.5";
let carregou = [false, false];

// Pegar usuario
fetch(apiGetMe, {
  method: "GET",
  headers: {
    "Content-type": "application/json",
    "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
    'Access-Control-Allow-Origin': '*'
  }
})
  .then(function (resposta) {
    return resposta.json();
  })
  .then(function (data) {
    let foundUser = false;
    data.forEach(function (user) {
      if(user.email == sessionStorage.getItem("email"))
      {
        sessionStorage.setItem("userId", user.id);
        if (user.name)
          nomeUsu.innerText = user.name + " " + user.lastName;
        foundUser = true;
        carregou[0] = true;
        if (carregou[0] && carregou[1])
        {
          loader.style.visibility = "hidden";
          body.style.opacity = "1";
        }
      }
    });
    if(!foundUser)
    {
      alert("We ran into some problem, please login again");
      sessionStorage.removeItem("jwt");
      sessionStorage.removeItem("email");
      window.location.href = "index.html";
    }
  })
  .catch(function (erro) {
    console.log(erro);
    alert("We ran into some problem, please login again");
    sessionStorage.removeItem("jwt");
    sessionStorage.removeItem("email");
    window.location.href = "index.html";
  });

// Botao de fechar sessao
closeApp.addEventListener("click", function () {
  sessionStorage.removeItem("jwt");
  sessionStorage.removeItem("email");
  window.location.href = "index.html";
});

fetch(apiTarefas, {
  method: "GET",
  headers: {
    "Content-type": "application/json",
    "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
    'Access-Control-Allow-Origin': '*'
  }
})
  .then(function (resposta) {
    return resposta.json();
  })
  .then(function (data) {
    let userTasks = [];
    data.forEach(function (task) {
      if(task.user.id == sessionStorage.getItem("userId"))
        userTasks.push(task);
    });

    userTasks.forEach(function (task) {
      let testIni = document.getElementsByClassName("container");
      if (testIni[0].id == "skeleton") {
        testIni[0].innerHTML = "";
        testIni[0].id = "";
      }

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
      let div3 = document.createElement("div");
      let p = document.createElement("p");
      let p2 = document.createElement("p");
      let p3 = document.createElement("p");
      
      div2.classList.add("descricao");
      p.classList.add("nome");
      p.innerText = task.description;
      div2.appendChild(p);

      div3.classList.add("timestamps");
      p2.classList.add("timestamp");
      p2.innerText = "Created in " + task.createdAt;
      div3.appendChild(p2);

      if ((task.finishedAt != null) && (task.completed == true))
      {
        p3.classList.add("timestamp");
        p3.innerText = "Finished at " + task.finishedAt;

        div3.appendChild(p3);
      }
      div2.appendChild(div3);

      liTarefas.appendChild(div2);
	  
      let div4 = document.createElement("div");
      div4.classList.add("not-done");
      div4.classList.add("delete");

      liTarefas.appendChild(div4);

      container[0].appendChild(liTarefas);

      div.addEventListener("click", function () {
        loader.style.visibility = "visible";
        body.style.opacity = "0.5";

        if (div.classList.contains("not-done")) {
          div.classList.remove("not-done");
          div.classList.add("done");

          let tarefa = {
            description: task.description,
            createdAt: task.createdAt,
            finishedAt: new Date().toLocaleDateString('en-US'),
            completed: true,
            userId: sessionStorage.getItem("userId")
          };

          path = apiTarefas + "/" + task.id;

          fetch(path, {
            method: "PUT",
            headers: {
              "Content-type": "application/json",
              "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
              'Access-Control-Allow-Origin': '*'
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
              alert("We ran into some problem, please try again");
              loader.style.visibility = "hidden";
              body.style.opacity = "1";
            });
        } else {
          div.classList.remove("done");
          div.classList.add("not-done");

          let tarefa = {
            description: task.description,
            createdAt: task.createdAt,
            finishedAt: null,
            completed: false,
            userId: sessionStorage.getItem("userId")
          };

          path = apiTarefas + "/" + task.id;

          fetch(path, {
            method: "PUT",
            headers: {
              "Content-type": "application/json",
              "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
              'Access-Control-Allow-Origin': '*'
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
              alert("We ran into some problem, please try again");
              loader.style.visibility = "hidden";
              body.style.opacity = "1";
            });
        }
      });
	  
      // Deletar Tarefa
      div4.addEventListener("click", function () {
        loader.style.visibility = "visible";
        body.style.opacity = "0.5";

        path = apiTarefas + "/" + task.id;

        fetch(path, {
          method: "DELETE",
          headers: {
            "Content-type": "application/json",
            "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
            'Access-Control-Allow-Origin': '*'
          }
        })
          .then(function (resposta) {
            document.location.reload(true)
            return resposta.json();
          })
          .catch(function (erro) {
            console.log(erro);
            alert("We ran into some problem, please try again");
            loader.style.visibility = "hidden";
            body.style.opacity = "1";
          });
      });
    });
    carregou[1] = true;
    if (carregou[0] && carregou[1])
    {
      loader.style.visibility = "hidden";
      body.style.opacity = "1";
    }
  })
  .catch(function (erro) {
    console.log(erro);
    alert("We ran into some problem, please login again");
    sessionStorage.removeItem("jwt");
    sessionStorage.removeItem("email");
    window.location.href = "index.html";
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

  if ((novaTarefa.value != "") && (novaTarefa.value.length <= 100))
  {
    loader.style.visibility = "visible";
    body.style.opacity = "0.5";

    let tarefa = {
      description: novaTarefa.value,
      createdAt: new Date().toLocaleDateString('en-US'),
      completed: status,
      userId: sessionStorage.getItem("userId")
    };

    fetch(apiTarefas, {
      method: "POST",
      headers: {
        "Content-type": "application/json",
        "Authorization": "Bearer " + sessionStorage.getItem("jwt"),
        'Access-Control-Allow-Origin': '*'
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
        alert("We ran into some problem, please try again");
        loader.style.visibility = "hidden";
        body.style.opacity = "1";
      });
  } 
  else if(novaTarefa.value.length > 100)
  {
    alert("Your task name needs to have no more than 100 characters!");
  }
  else
  {
    alert("You need to give a name to your task!");
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

// Listener de contagem de caracteres
novaTarefa.addEventListener("keyup", function(){
  spanChar.innerText = 100 - novaTarefa.value.length + " characters left";
});