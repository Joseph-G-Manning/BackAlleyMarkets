# BackAlleyMarkets

Inventory tracker for a fictional store that offers different goods and categories.

## Project Structure

- `BackAlleyMarketsApi/` - ASP.NET Core Web API (Backend)
- `backalleymarkets-client/` - Vite + React (Frontend)

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js (v18+ recommended)](https://nodejs.org/)
- [npm](https://www.npmjs.com/) (comes with Node.js)

---

## Running the Backend (API)

1. Open a terminal and navigate to the API project directory:

    ```sh
    cd BackAlleyMarketsApi/src/BackAlleyMarketsApi
    ```

2. Restore dependencies:

    ```sh
    dotnet restore
    ```

3. Run the API:

    ```sh
    dotnet run
    ```

   The API will start (by default on `https://localhost:7111` and `http://localhost:5142`).

---

## Running the Frontend (Client)

1. Open a new terminal and navigate to the client directory:

    ```sh
    cd backalleymarkets-client
    ```

2. Install dependencies:

    ```sh
    npm install
    ```

3. Start the development server:

    ```sh
    npm run dev
    ```

   The app will be available at [http://localhost:3000](http://localhost:3000).

---

## Notes

- Make sure both the API and client are running for full functionality.
- You may need to configure the frontend to point to the correct API URL (check `.env` or API calls in the frontend code).
- To view the API documentation in OpenAPI (Swagger UI), navigate to `https://localhost:7111/openapi/v1.json` or `http://localhost:5142/openapi/v1.json` in your browser while the API is running.

---

## License

MIT License. See [LICENSE](LICENSE) for details.
