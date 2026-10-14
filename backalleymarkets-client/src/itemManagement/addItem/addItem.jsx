import React, { useState, useEffect } from "react";
import "./addItem.css";
import { useParams, useLocation, useNavigate } from "react-router";

const AddItem = () => {
  const location = useLocation();

  let navigate = useNavigate();

  const navItemList = () => {
    navigate("/");
  };

  return (
    <>
      <div className="card">
        <h3>Add New Item</h3>
        <div>
          <label htmlFor="itemName">Item Name:</label>
          <input
            type="text"
            class="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
          />
        </div>
        <div>
          <label htmlFor="itemCategory">Item Category:</label>
          <input
            type="text"
            class="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
          />
        </div>
                <div>
          <label htmlFor="tags">Tags:</label>
          <input
            type="text"
            class="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
          />
        </div>

        <button
          className="bg-indigo-500 hover:bg-fuchsia-500"
          onClick={navItemList}
        >
          Return to List
        </button>
      </div>
    </>
  );
};

export default AddItem;
