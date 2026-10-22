import React, { useState, useEffect } from "react";
import "./addItem.css";
import { useParams, useLocation, useNavigate } from "react-router";

const TAG_PATTERN =
  /^[\p{L}\p{M}\p{N}_]+(-[\p{L}\p{M}\p{N}_]+)*( *, *[\p{L}\p{M}\p{N}_]+(-[\p{L}\p{M}\p{N}_]+)*)*$/u;

function parseTags(value) {
  return value.trim().split(/,\s*/).filter(Boolean);
}

const AddItem = () => {
  const [itemName, setItemName] = useState("");
  const [itemDescription, setItemDescription] = useState("");
  const [itemCategory, setItemCategory] = useState("");
  const [itemTags, setItemTags] = useState("");
  const [tagsTouched, setTagsTouched] = useState(false);
  // const [isTagsValid, setIsTagsValid] = useState(false);
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

  const tagsTrimmed = itemTags.trim();
  const isTagsEmpty = tagsTrimmed === "";
  let isTagsValid = TAG_PATTERN.test(tagsTrimmed);

  let error = "";
  if (tagsTouched) {
    if (isTagsEmpty) {
      error = "Please enter at least one tag.";
    } else if (!isTagsValid) {
      error =
        "Tags must be single words (no spaces), separated by commas. Example: Shoes, Street_Shoes";
    }
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
    setTagsTouched(false);
    isTagsValid = false;
  }

  return (
    <>
      <div className="card">
        <h3>Add New Item</h3>
        <form onSubmit={handleItemSubmit} noValidate>
          <div className="flex flex-col gap-2 mb-4">
            <label
              htmlFor="itemName"
              className="flex justify-start font-semibold text-gray-700"
            >
              Item Name
            </label>
            <input
              type="text"
              value={itemName}
              onChange={(e) => setItemName(e.target.value)}
              className="p-3 border border-gray-300 rounded-md focus:outline-none"
            />
          </div>
          <div className="flex flex-col gap-2 mb-4">
            <label
              htmlFor="itemDescription"
              className="flex justify-start font-semibold text-gray-700"
            >
              Item Description
            </label>
            <input
              type="text"
              value={itemDescription}
              onChange={(e) => setItemDescription(e.target.value)}
              className="p-3 border border-gray-300 rounded-md focus:outline-none"
            />
          </div>
          <div className="flex flex-col gap-2 mb-4">
            <label
              htmlFor="itemCategory"
              className="flex justify-start font-semibold text-gray-700"
            >
              Item Category
            </label>
            <input
              type="text"
              value={itemCategory}
              onChange={(e) => setItemCategory(e.target.value)}
              className="p-3 border border-gray-300 rounded-md focus:outline-none"
            />
          </div>
          <div className="flex flex-col gap-2 mb-4">
            <label
              htmlFor="tags"
              className="flex justify-start font-semibold text-gray-700"
            >
              Tags
            </label>
            <input
              type="text"
              value={itemTags}
              onChange={(e) => setItemTags(e.target.value)}
              onBlur={() => setTagsTouched(true)}
              aria-invalid={!!error}
              aria-describedby={error ? "tags-error" : undefined}
              className={`p-3 border border-gray-300 rounded-md focus:outline-none ${
                error
                  ? "border-red-500 focus:ring-red-300"
                  : "border-gray-300 focus:ring-blue-300"
              }`}
            />
            {error && (
              <p id="tags-error" role="alert" className="text-sm text-red-600">
                {error}
              </p>
            )}
          </div>

          <button
            className="bg-indigo-500 hover:bg-fuchsia-500 disabled:opacity-50"
            type="submit"
            disabled={tagsTouched && (isTagsEmpty || !isTagsValid)}
          >
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
