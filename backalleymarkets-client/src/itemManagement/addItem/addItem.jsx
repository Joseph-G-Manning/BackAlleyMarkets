import React, { useState, useEffect } from "react";
import "./addItem.css";
import { useParams, useLocation, useNavigate } from "react-router";

const TAG_PATTERN =
  /^[\p{L}\p{M}\p{N}_]+(-[\p{L}\p{M}\p{N}_]+)*( *, *[\p{L}\p{M}\p{N}_]+(-[\p{L}\p{M}\p{N}_]+)*)*$/u;

function itemTagsArray(value) {
  return value.trim().split(/,\s*/).filter(Boolean);
}

const REQUIRED_FIELDS = [
  { name: "itemName", label: "Item Name" },
  { name: "itemDescription", label: "Item Description" },
  { name: "categoryName", label: "Item Category" },
];

const INITIAL_VALUES = {
  ...Object.fromEntries(REQUIRED_FIELDS.map((f) => [f.name, ""])),
  itemTags: "",
};

const INITIAL_TOUCHED = Object.fromEntries(
  Object.keys(INITIAL_VALUES).map((k) => [k, false]),
);

const AddItem = () => {
  const [values, setValues] = useState(INITIAL_VALUES);
  const [touched, setTouched] = useState(INITIAL_TOUCHED);
  const apiUrl = "http://localhost:5142/api/Items/add_item";
  const location = useLocation();

  let navigate = useNavigate();

  const navItemList = () => {
    navigate("/");
  };

  function handleChange(e) {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
    setTouched((prev) => ({ ...prev, [name]: true }));
  }

  function handleBlur(e) {
    const { name } = e.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
  }

  // Per-field errors (only shown once the field is touched)
  const errors = {};
  for (const { name, label } of REQUIRED_FIELDS) {
    if (touched[name] && values[name].trim() === "") {
      errors[name] = `${label} is required.`;
    }
  }
  const tagsTrimmed = values.itemTags.trim();
  if (touched.itemTags) {
    if (tagsTrimmed === "") {
      errors.itemTags = "Please enter at least one tag.";
    } else if (!TAG_PATTERN.test(tagsTrimmed)) {
      errors.itemTags =
        "Tags must be single words (no spaces), separated by commas. Example: Shoes, Street_Shoes";
    }
  }

  // Button is enabled only when every field is touched AND valid
  const allTouched = Object.values(touched).every(Boolean);
  const allFilled = REQUIRED_FIELDS.every((f) => values[f.name].trim() !== "");
  const tagsValid = tagsTrimmed !== "" && TAG_PATTERN.test(tagsTrimmed);
  const canSubmit = allTouched && allFilled && tagsValid;

  const handleItemSubmit = async (e) => {
    e.preventDefault();
    if (!canSubmit) return; // safety net (e.g. Enter key)
    try {
      console.log("Item Name: ", values.itemName);
      console.log("Item Description: ", values.itemDescription);
      console.log("Item Category: ", values.categoryName);
      console.log("Item Tags: ", values.itemTags);
      
      const result = await addItem({
        ...Object.fromEntries(
          REQUIRED_FIELDS.map((f) => [f.name, values[f.name].trim()]),
        ),
        itemTags: itemTagsArray(values.itemTags),
      });

      console.log("Result from addItem: ", result);
      setValues(INITIAL_VALUES);
      setTouched(INITIAL_TOUCHED);
    } catch (error) {
      console.error("Error submitting item: ", error);
    }
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

  const inputClass = (hasError) =>
    `w-full rounded border px-3 py-2 outline-none focus:ring-2 ${
      hasError
        ? "border-red-500 focus:ring-red-300"
        : "border-gray-300 focus:ring-blue-300"
    }`;

    return (
    <>
      <div className="card">
        <h3>Add New Item</h3>
        <form onSubmit={handleItemSubmit} noValidate className="space-y-4">
          {REQUIRED_FIELDS.map(({ name, label }) => (
            <div key={name} className="flex flex-col gap-2 mb-4">
              <label
                htmlFor={name}
                className="flex justify-start font-semibold text-gray-700"
              >
                {label}
              </label>
              <input
                id={name}
                name={name}
                type="text"
                value={values[name]}
                onChange={handleChange}
                onBlur={handleBlur}
                aria-invalid={!!errors[name]}
                aria-describedby={errors[name] ? `${name}-error` : undefined}
                className={inputClass(errors[name])}
              />
              {errors[name] && (
                <p
                  id={`${name}-error`}
                  role="alert"
                  className="text-sm text-red-600"
                >
                  {errors[name]}
                </p>
              )}
            </div>
          ))}
          <div className="flex flex-col gap-2 mb-4">
            <label htmlFor="itemTags" className="flex justify-start font-semibold text-gray-700">
              Tags
            </label>
            <input
              id="itemTags"
              name="itemTags"
              type="text"
              value={values.itemTags}
              onChange={handleChange}
              onBlur={handleBlur}
              aria-invalid={!!errors.itemTags}
              aria-describedby={errors.itemTags ? "itemTags-error" : undefined}
              className={inputClass(errors.itemTags)}
            />
            {errors.itemTags && (
              <p
                id="itemTags-error"
                role="alert"
                className="text-sm text-red-600"
              >
                {errors.itemTags}
              </p>
            )}
          </div>

          <button
            type="submit"
            disabled={!canSubmit}
            className="bg-indigo-500 hover:bg-fuchsia-500 disabled:cursor-not-allowed disabled:opacity-50 disabled:hover:none"
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
