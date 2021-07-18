# CaPPMS - [Capstone Project Management System](https://umgc-cappms.azurewebsites.net)

You've reached the UMGC Project management project!

This project was first started in design by the Fall 2020 class and moved a little further by the Summer 2021 DevSecOps team.

## Introduction
The project idea website allows external parties to request a software development project for the SWEN 670 course. The website has a log-in function where faculty will be able to review, approve, or deny requests. It was developed using the Microsoft Blazor framework, and runs on Microsoft Azure.
Accessing the website
The website can be accessed on https://umgc-cappms.azurewebsites.net. 

## Submit an Idea
Project idea website

Submitting an idea
The initial page that the user sees contains a form. Here the user can fill out their name, email, project title and description, and upload attachments.

![image](https://user-images.githubusercontent.com/4316475/126079709-011d9ee0-bd83-4fdd-88bb-0f412ad77145.png)

## Managing the Project List

**Note**: This is an authorized user only area that was meant to be used by the faculity members of UMGC.

Clicking the Project List link from the Navigation Bar will bring you to the Project List Page:

![image](https://user-images.githubusercontent.com/4316475/126079932-56ddc0fa-004b-4100-be8a-6c3e2db1f229.png)

The initial page displays a list of projects currently being tracked by the system. For more detail click the `View` button for the desired item of interest.
This will display more information about about the project such as the person of contact information, attachements that were uploaded, and any comments made by other authroized users. There are some menu buttons at the bottom of the page as well that allow actions to export the project, close the project details window, approve the project, and delete the project. If there was some update to some of the fields by the authorized user, an update button will also be added allowing the change to propigate throughout the system.

![image](https://user-images.githubusercontent.com/4316475/126080053-6e79d456-45a4-46ca-a9ae-75edca520b29.png)

The export button dynamically creates a PDF file with information from the form and adds links for the attachments to be downloaded.
![image](https://user-images.githubusercontent.com/4316475/126080081-1207a490-2f3a-482b-b0e6-2f28235d0452.png)
