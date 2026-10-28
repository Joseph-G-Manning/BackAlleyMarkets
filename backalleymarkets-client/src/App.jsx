import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router";
import "./App.css";
import Inventory from "./inventory/inventory";
import ItemDetail from "./itemDetail/itemDetail";
import AddItem from "./itemManagement/addItem/addItem";

function App() {
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
