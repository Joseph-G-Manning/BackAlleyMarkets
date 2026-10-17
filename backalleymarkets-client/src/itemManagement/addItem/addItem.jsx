import React, { useState, useEffect } from "react";
import "./addItem.css";
import { useParams, useLocation, useNavigate } from "react-router";

const AddItem = () => {
  const [itemName, setItemName] = useState("");
  const [itemDescription, setItemDescription] = useState("");
  const [itemCategory, setItemCategory] = useState("");
  const [itemTags, setItemTags] = useState("");
  const apiUrl = "http://localhost:5142/api/Items/add_item";
  const location = useLocation();

  let navigate = useNavigate();

  const navItemList = () => {
    navigate("/");
  };

  async function addItem(itemData) {
    const res = await fetch(apiUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(itemData),
    });

    if (!res.ok) {
      throw new Error(`Request failed: ${res.status}`);
    }
    return res.json();
  }

  const handleItemSubmit = async (e) => {
    e.preventDefault();
    try {
      console.log("Item Name: ", itemName);
      console.log("Item Description: ", itemDescription);
      console.log("Item Category: ", itemCategory);
      console.log("Item Tags: ", itemTags);

      const itemTagsArray = itemTags.split(",").map((tag) => tag.trim());
      console.log("Item Tags Array: ", itemTagsArray);

      const result = await addItem({
        itemName: itemName,
        itemDescription: itemDescription,
        categoryName: itemCategory,
        itemTags: itemTagsArray,
      });
      console.log("Result from addItem: ", result);
      clearForm();
    } catch (error) {
      console.error("Error submitting item: ", error);
    }
  };

  function clearForm() {
    setItemName("");
    setItemDescription("");
    setItemCategory("");
    setItemTags("");
  }

  return (
    <>
      <div className="card">
        <h3>Add New Item</h3>
        <form onSubmit={handleItemSubmit}>
          <div>
            <label htmlFor="itemName">Item Name:</label>
            <input
              type="text"
              value={itemName}
              onChange={(e) => setItemName(e.target.value)}
              className="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
            />
          </div>
          <div>
            <label htmlFor="itemDescription">Item Description:</label>
            <input
              type="text"
              value={itemDescription}
              onChange={(e) => setItemDescription(e.target.value)}
              className="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
            />
          </div>
          <div>
            <label htmlFor="itemCategory">Item Category:</label>
            <input
              type="text"
              value={itemCategory}
              onChange={(e) => setItemCategory(e.target.value)}
              className="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
            />
          </div>
          <div>
            <label htmlFor="tags">Tags:</label>
            <input
              type="text"
              value={itemTags}
              onChange={(e) => setItemTags(e.target.value)}
              className="invalid:border-pink-500 invalid:text-pink-600 focus:border-sky-500 focus:outline focus:outline-sky-500 focus:invalid:border-pink-500 focus:invalid:outline-pink-500 disabled:border-gray-200 disabled:bg-gray-50 disabled:text-gray-500 disabled:shadow-none dark:disabled:border-gray-700 dark:disabled:bg-gray-800/20 ..."
            />
          </div>

          <button className="bg-indigo-500 hover:bg-fuchsia-500" type="submit">
            Submit
          </button>
        </form>
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
