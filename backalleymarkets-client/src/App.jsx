import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router";
import "./App.css";
import Inventory from "./inventory/inventory";
import ItemDetail from "./itemDetail/itemDetail";
import AddItem from "./itemManagement/addItem/addItem";
// import reactLogo from "./assets/react.svg";
// import viteLogo from "/vite.svg";

function App() {
  // const [count, setCount] = useState(0);
  // const [items, setItems] = useState([]);

  // useEffect(() => {
  //   //   const getItems = async () => {}
  //   // }, []);
  //   // async function getItems() {
  //   const initGetItems = async () => {
  //     try {
  //       // setItems(items => items = []);
  //       const res = await fetch("http://localhost:5142/api/Items/");
  //       const data = await res.json();
  //       console.log("Returnded data: ", data);
  //       console.log("Items before: ", items);

  //       // setItems(items => items = data);
  //       setItems(data);
  //       console.log("Items after: ", items);
  //     } catch (error) {
  //       console.error("Error fetching data: ", error);
  //     }
  //   };

  //   initGetItems();
  // }, []);

  // async function getItems() {
  //   try {
  //     // setItems(items => items = []);
  //     const res = await fetch("http://localhost:5142/api/Items/");
  //     const data = await res.json();
  //     console.log("Returnded data: ", data);
  //     console.log("Items before: ", items);

  //     // setItems(items => items = data);
  //     setItems([]);
  //     console.log("Items after: ", items);
  //   } catch (error) {
  //     console.error("Error fetching data: ", error);
  //   }
  // }

  return (
    <>
      <Router>
        <h1>Back Alley Markets</h1>
        <div className="container">
          <Routes>
            <Route index element={<Inventory />} />
            <Route path="/items/:id" element={<ItemDetail />} />
            <Route path="/addItem" element={<AddItem />} />
          </Routes>
        </div>
      </Router>
    </>
  );
}

export default App;
