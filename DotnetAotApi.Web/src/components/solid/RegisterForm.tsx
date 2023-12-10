import LoginFormBase, { type LoginSchema } from "./LoginFormBase.tsx";

async function registerUser(user: LoginSchema) {
  try {
    const response = await fetch("/v1.0/auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json,application/problem+json",
      },
      body: JSON.stringify(user),
    });
    console.log("response", response);

    if (response.status === 201) {
      window.location.assign("/login");
      return undefined;
    }

    if (response.status === 400) {
      const body = await response.json();
      console.log("body", body);
      return body.errors as Record<string, string[]> | undefined;
    }

    console.error("Unhandled error from API", response);
  } catch (err) {
    console.log("err", err);
  }
}

export default function RegisterForm() {
  return <LoginFormBase onSubmit={registerUser} />;
}
