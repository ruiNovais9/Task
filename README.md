# Docker

You need to have docker installed and running it.

Open a CMD (Command Prompt) in the folder (you should be on same path of docker-compose.yml) and write:

docker-compose up --build

With that is created a image and execute a container with the name Task-Manager.

After this you can open the browser and put: http://localhost:5000/swagger and use swagger to do the request, or can use a postman to the url: http://localhost:5000/Projects and use the service you want.

# How run locally both projects

Open the folder API and open the solution TaskManager.sln with visual studio 2022.

For front End open visual code on folder Angular, and write on console npm install and next ng serve or npm start.

# Unit tests

For front end i used karma, to run locally write ng test.

For back end i used XUnit, to run locally on visual studio go to tab test and click on Run all tests.