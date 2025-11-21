1) The Shell SPA and Products Microservice SPA use Next.js 14 with the app directory structure.

2) The NextJS SPA projects are built and exported ("npm run export") as static files under the ASP.NET Core 8.0 BFF. Please see the "out" folder where the final static files can be found.

3) The "Backend for Frontend" (BFF) server is implemented using ASP.NET Core 8.0. The BFF server serves the static files from the "out" folder mentioned above and handles API requests from the SPA's TypeScript code.

4) Once the .NET soltuion loads for the first time, you must configure "Configure Startup Projects..." to start multiple projects. The order in which they must run is as below:
	1. IDP
	2. Products Microservice API
	3. ... all other Microservice APIs
	4. Products Microservice BFF Frontend
	5. ... all other Microservice BFF Frontends
	5. PAS Shell BFF Frontend

5) Once the solution is run, go to "System Tray", locate the IIS Express icon, and locate the Shell BFF server (https). Click the URL with https:// to launch the Shell SPA Frontend in your default web browser.

6) Since the Shell BFF project is protected by the IDP, the browser will redirect you to the IDP login page. Enter "JuliaRob" and "JuliaRob123" as credentials to log in. User Julia Roberts has the highest number of role claims.

7) After successful login at the IDP login page, the browser will be redirected back to the Shell BFF Frontend application, and the browser will show the landing page of the Shell BFF Frontend.

8) Based on the signed in user's roles (associated with the credentials), the left pane will show the available "Microservices", and various "Management Areas" under the Microservices, and under each "Management Area", you will see various "Menu Item" which takes you to the screen that does that task.

8) Only the Products microservice has been built as on 21-NOV-2025 (in NextJS). The Payments microservice shall be in ReactJS, and the Orders microservice will be in ASP.NET Core 8.0 Blazer.

9) The sign out from the Shell BFF application shall sign out the user from all involved Microservice BFF Frontends relevant for the browser.

10) Known issues:
   - When logged out of the PAS system, the Products microservice's default route is still accessible. All other functional routes are secured with "Authentication required" message.
   - When you click on a menu item for which a NextJS component page is not available, it shows empty white screen in the content area (not a bug, but a graceful message is not yet implemented).

11) Final touches required (not urgent, only after all modules are implemented):
   - A "consent" page must be implemented in the IDP to ask users for consent before sharing their profile data with the BFF UIs and APIs.
   - Teraform scripts must be written to provision all required infrastructure in a cloud provider (e.g., Azure, AWS, GCP).

12) Important Gotchas and relevant URLs:
	- Cookie names of BFF projects must begin with "__". If not, there can be issues with signing in and out.
		- Shell BFF UI cookie name: "__PAS-Shell-Host-bff"
		- Products BFF UI cookie name: "__PAS-Microservice-Products-Host-bff"
