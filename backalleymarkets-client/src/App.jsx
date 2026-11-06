import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router";
import "./App.css";
import Inventory from "./inventory/inventory";
import ItemDetail from "./itemDetail/itemDetail";
import AddItem from "./itemManagement/addItem/addItem";
import Footer from "./layoutComponents/footer/footer";
import Header from "./layoutComponents/header/header";

function App() {
  return (
    <>
      <Router>
        <Header />
        <div className="container">
          <Routes>
            <Route index element={<Inventory />} />
            <Route path="/items/:id" element={<ItemDetail />} />
            <Route path="/addItem" element={<AddItem />} />
          </Routes>
        </div>
        <Footer />
      </Router>
    </>
  );
}

export default App;
