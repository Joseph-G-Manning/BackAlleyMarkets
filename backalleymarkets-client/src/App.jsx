import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes } from 'react-router';
import "./App.css";
import Inventory from "./inventory/inventory";
import ItemDetail from "./itemDetail/itemDetail";
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
        {/* <div className="card">
        <button
          className="bg-indigo-500 hover:bg-fuchsia-500 rotate-45"
          onClick={() => getItems()}
        >
          Click for Items
        </button>
        <div className="grid grid-cols-4 gap-4">
          {items.map((item) => (
            <div key={item.itemId}>{item.itemName}</div>
          ))}
        </div>
      </div> */}
        <div className="container">
          <Routes>
            <Route index element={<Inventory />} />
            <Route path="/items/:id" element={<ItemDetail />} />
            {/* <Route exact path="/item/:username" component={Profile} /> */}
            {/* <Route component={NoMatch} /> */}
          </Routes>
        </div>
        {/* <Inventory /> */}
      </Router>
    </>
  );
}

export default App;
