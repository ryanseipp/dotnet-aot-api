import { Show, type JSX } from "solid-js";
import { createStore } from "solid-js/store";
import { z } from "zod";

const LoginSchema = z.object({
  username: z
    .string({ required_error: "Username is required." })
    .min(1, "Username is required.")
    .max(256, "Username cannot be more than 256 characters."),
  password: z
    .string({ required_error: "Password is required." })
    .min(16, "Password must be at least 16 characters.")
    .max(64, "Password cannot be more than 64 characters"),
});
export type LoginSchema = z.infer<typeof LoginSchema>;

type Props = {
  onSubmit: (
    user: LoginSchema,
  ) => Promise<Record<string, string[]> | undefined>;
};

export default function LoginFormBase({ onSubmit }: Props) {
  const [errors, setErrors] = createStore<Record<string, string>>({});

  const handleSubmit: JSX.EventHandler<HTMLFormElement, Event> = async (e) => {
    e.preventDefault();
    const data = new FormData(e.currentTarget);
    const parseResult = LoginSchema.safeParse(
      Object.fromEntries(data.entries()),
    );

    if (parseResult.success) {
      const errors = await onSubmit(parseResult.data);
      if (errors) {
        const formattedErrors = Object.fromEntries(
          Object.entries(errors).map(([name, value]) => [
            name.toLowerCase(),
            value?.join(", "),
          ]),
        );
        setErrors(formattedErrors);
      }
      return;
    }

    const errors = Object.fromEntries(
      Object.entries(parseResult.error.flatten().fieldErrors).map(
        ([name, value]) => [name, value?.join(", ")],
      ),
    );
    setErrors(errors);
  };

  const validateOnBlur: JSX.FocusEventHandler<HTMLInputElement, FocusEvent> = (
    e,
  ) => {
    const { value, name } = e.target;
    const parseResult = LoginSchema.pick({ [name]: true }).safeParse({
      [name]: value,
    });

    if (!parseResult.success) {
      const errors =
        parseResult.error.flatten().fieldErrors[name as keyof LoginSchema];
      setErrors({ [name]: errors?.join(", ") });
    }
  };

  const clearErrorOnInput: JSX.InputEventHandler<
    HTMLInputElement,
    InputEvent
  > = (e) => {
    const { name } = e.target;
    if (!errors[name]) return;

    setErrors({ [name]: undefined });
  };

  return (
    <form class="flex flex-col gap-4 w-full" novalidate onSubmit={handleSubmit}>
      <div class="flex flex-col">
        <label for="username" class="block">
          Username<span class="text-red-600">*</span>
        </label>
        <input
          type="text"
          id="username"
          name="username"
          required
          minlength="1"
          maxlength="256"
          class="rounded p-1 dark:bg-slate-950 border border-slate-800"
          onBlur={validateOnBlur}
          onInput={clearErrorOnInput}
        />
        <Show when={errors.username}>
          <span class="text-red-600">{errors.username}</span>
        </Show>
      </div>
      <div class="flex flex-col">
        <label for="password" class="block">
          Password<span class="text-red-600">*</span>
        </label>
        <input
          type="password"
          id="password"
          name="password"
          required
          minlength="16"
          class="rounded p-1 dark:bg-slate-950 border border-slate-800"
          onBlur={validateOnBlur}
          onInput={clearErrorOnInput}
        />
        <Show when={errors.password}>
          <span class="text-red-600">{errors.password}</span>
        </Show>
        <Show
          when={
            errors.password !== undefined && errors.password.includes("breach")
          }
        >
          <span class="text-xs mt-1">
            This service utilizes{" "}
            <a href="https://haveibeenpwned.com/Passwords" class="underline">
              HaveIBeenPwned
            </a>{" "}
            to check if the password entered has been found in a data breach. If
            so, the password is more vulnerable to dictionary attacks. The
            password entered is never sent to this service.
          </span>
        </Show>
      </div>
      <button
        type="submit"
        class="shadow shadow-slate-950 p-2 bg-slate-200 dark:bg-slate-700 rounded font-bold disabled:bg-gray-200 disabled:dark:bg-gray-900 disabled:text-gray-500 disabled:shadow-none"
        disabled={Boolean(errors.username || errors.password)}
      >
        Register
      </button>
    </form>
  );
}
