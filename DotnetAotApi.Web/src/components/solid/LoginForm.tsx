import LoginFormBase, { type LoginSchema } from "./LoginFormBase.tsx";

async function loginUser(user: LoginSchema) {
  try {
    const response = await fetch("/v1.0/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json,application/problem+json",
      },
      body: JSON.stringify(user),
    });
    console.log("response", response);

    if (response.status === 200) {
      window.location.assign("/v1.0/auth/current");
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

export default function LoginForm() {
  return <LoginFormBase onSubmit={loginUser} />;
}
