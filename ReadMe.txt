1) The SPA uses Next.js 14 with the app directory structure.

2) The SPA project is already compiled, bundled and exported using Next.js build tools ("npm run export"). Look at the "out" folder for the final static files.

3) The "Backend for Frontend" (BFF) server is implemented using ASP.NET Core 8.0. The BFF server serves the static files from the "out" folder and handles API requests.

4) Once the .NET soltuion loads, you must configure "Configure Startup Projects..." to start both the BFF server project and the SPA project.
   This will ensure that both the backend and frontend are running simultaneously during development."

5) Once the solution is started, go to "System Tray" and find the icon for the BFF server. Right-click on it and select "Open Web Application" to launch the SPA in your default web browser.

6) Since the BFF project is protected by the IDP, the browser will redirect you to the IDP login page. Enter "alice" and "alice" as credentials to log in.

7) After successful login, you will be redirected back to the SPA application, and the browser will show the landing page of the BFF web application.

8) The "microservice" modules - Orders, Payment and Products - are NOT implemented yet. Those buttons would not show anything.

9) The sign out from the BFF application, click on the "Signout" button at the top right corner. It would sign you out from both the BFF application and the IDP.

10) What is not yet implemented:
   - The microservice modules (Orders, Payment, Products) are not implemented yet.
	  - For each module:
		- A BFF Web UI (i.e., ASP.NET Core 8.0 and NextJS), protected by the IDP.
		- An API application using ASP.NET Core 8.0, protected by the IDP.
	  - To begin with, let's implement the "Products" module first.

11) Final touches required (not urgent, only after all modules are implemented):
   - Users and roles must come from a database (e.g., SQL Server) instead of being hardcoded in the IDP.
   - A "common" project that has the URLs of all applications involved (IDP, BFF UIs and APIs) must be created and used by all projects to avoid hardcoding URLs in multiple places.
   - A "consent" page must be implemented in the IDP to ask users for consent before sharing their profile data with the BFF UIs and APIs.
   - Terform scripts must be created to provision all required infrastructure in a cloud provider (e.g., Azure, AWS, GCP).

12) Cookie names of BFF projects:
   - Shell BFF UI: "__PAS-Shell-Host-bff"
   - Products BFF UI: "__PAS-Microservice-Products-Host-bff"

