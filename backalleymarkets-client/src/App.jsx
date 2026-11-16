import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router";
import "./App.css";
import CreateUserProfileForm from "./components/userComponents/createUserProfileComponent/CreateUserProfile";
import Inventory from "./inventory/inventory";
import ItemDetail from "./components/itemComponents/itemDetail/itemDetail";
import AddItem from "./components/itemComponents/itemManagement/addItem/addItem";
import Footer from "./components/layoutComponents/footer/footer";
import Header from "./components/layoutComponents/header/header";

function App() {
  return (
    <>
      <Router>
        <Header />
        <div className="container">
          <Routes>
            <Route path="/createUserProfile" element={<CreateUserProfileForm />} />
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
