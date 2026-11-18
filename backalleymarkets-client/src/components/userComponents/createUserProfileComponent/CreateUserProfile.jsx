import { useState } from "react";

const MAX_LENGTH = 50;

const initialValues = {
  firstName: "",
  lastName: "",
  userName: "",
  userPassword: "",
};

function Field({ id, label, error, children }) {
  return (
    <div className="flex flex-col gap-1.5">
      <label htmlFor={id} className="text-sm font-medium text-slate-700">
        {label}
      </label>
      {children}
      {error && (
        <p id={`${id}-error`} className="text-sm text-red-600">
          {error}
        </p>
      )}
    </div>
  );
}

const inputClasses = (hasError) =>
  [
    "w-full rounded-md border bg-white px-3 py-2 text-slate-900 shadow-sm",
    "placeholder:text-slate-400",
    "focus:outline-none focus:ring-2 focus:ring-offset-0",
    hasError
      ? "border-red-500 focus:ring-red-500"
      : "border-slate-300 focus:border-indigo-600 focus:ring-indigo-600",
  ].join(" ");

export default function CreateUserProfileForm({
  endpoint = "http://localhost:5142/api/user-profile",
  onSuccess,
}) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState({});
  const [submitError, setSubmitError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [created, setCreated] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setValues((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: undefined }));
    }
  };

  const validate = () => {
    const next = {};
    const first = values.firstName.trim();
    const last = values.lastName.trim();
    const user = values.userName.trim();

    if (!first) next.firstName = "Enter a first name.";
    if (!last) next.lastName = "Enter a last name.";
    if (!user) next.userName = "Enter a username.";
    if (!values.userPassword) next.userPassword = "Enter a password.";

    return next;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitError("");
    setCreated(false);

    const validationErrors = validate();
    setErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    setSubmitting(true);
    try {
      const response = await fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          firstName: values.firstName.trim(),
          lastName: values.lastName.trim(),
          userName: values.userName.trim(),
          userPassword: values.userPassword,
        }),
      });

      if (response.status === 409) {
        setErrors({ userName: "That username is already taken." });
        return;
      }

      if (!response.ok) {
        throw new Error("Request failed");
      }

      const data = await response.json().catch(() => null);
      setValues(initialValues);
      setShowPassword(false);
      setCreated(true);
      onSuccess?.(data);
    } catch {
      setSubmitError(
        "We couldn't create the profile. Check your connection and try again.",
      );
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="mx-auto w-full max-w-xl px-4 py-8 sm:px-6 sm:py-12">
      <form
        onSubmit={handleSubmit}
        noValidate
        className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm sm:p-8"
      >
        <h2 className="text-xl font-semibold text-slate-900 sm:text-2xl">
          Create your profile
        </h2>
        <p className="mt-1 text-sm text-slate-600">
          Add your name and choose a username and password.
        </p>

        <div className="mt-6 grid grid-cols-1 gap-5 sm:grid-cols-2">
          <Field id="firstName" label="First name" error={errors.firstName}>
            <input
              id="firstName"
              name="firstName"
              type="text"
              autoComplete="given-name"
              maxLength={MAX_LENGTH}
              value={values.firstName}
              onChange={handleChange}
              aria-invalid={Boolean(errors.firstName)}
              aria-describedby={
                errors.firstName ? "firstName-error" : undefined
              }
              className={inputClasses(errors.firstName)}
            />
          </Field>

          <Field id="lastName" label="Last name" error={errors.lastName}>
            <input
              id="lastName"
              name="lastName"
              type="text"
              autoComplete="family-name"
              maxLength={MAX_LENGTH}
              value={values.lastName}
              onChange={handleChange}
              aria-invalid={Boolean(errors.lastName)}
              aria-describedby={errors.lastName ? "lastName-error" : undefined}
              className={inputClasses(errors.lastName)}
            />
          </Field>

          <div className="sm:col-span-2">
            <Field id="userName" label="Username" error={errors.userName}>
              <input
                id="userName"
                name="userName"
                type="text"
                autoComplete="username"
                maxLength={MAX_LENGTH}
                value={values.userName}
                onChange={handleChange}
                aria-invalid={Boolean(errors.userName)}
                aria-describedby={
                  errors.userName ? "userName-error" : undefined
                }
                className={inputClasses(errors.userName)}
              />
            </Field>
          </div>

          <div className="sm:col-span-2">
            <Field
              id="userPassword"
              label="Password"
              error={errors.userPassword}
            >
              <div className="relative">
                <input
                  id="userPassword"
                  name="userPassword"
                  type={showPassword ? "text" : "password"}
                  autoComplete="new-password"
                  maxLength={MAX_LENGTH}
                  value={values.userPassword}
                  onChange={handleChange}
                  aria-invalid={Boolean(errors.userPassword)}
                  aria-describedby={
                    errors.userPassword ? "userPassword-error" : undefined
                  }
                  className={`${inputClasses(errors.userPassword)} pr-16`}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((s) => !s)}
                  aria-pressed={showPassword}
                  className="absolute inset-y-0 right-0 rounded-r-md px-3 text-sm font-medium text-indigo-700 hover:text-indigo-900 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-600"
                >
                  {showPassword ? "Hide" : "Show"}
                </button>
              </div>
            </Field>
          </div>
        </div>

        {submitError && (
          <p
            role="alert"
            className="mt-5 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700"
          >
            {submitError}
          </p>
        )}

        {created && (
          <p
            role="status"
            className="mt-5 rounded-md border border-green-200 bg-green-50 px-3 py-2 text-sm text-green-800"
          >
            Profile created.
          </p>
        )}

        <button
          type="submit"
          disabled={submitting}
          className="mt-6 w-full rounded-md bg-indigo-600 px-4 py-2.5 font-medium shadow-sm transition-colors hover:bg-indigo-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-indigo-600 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60 sm:w-auto sm:px-6"
        >
          {submitting ? "Creating profile…" : "Create profile"}
        </button>
      </form>
    </div>
  );
}
