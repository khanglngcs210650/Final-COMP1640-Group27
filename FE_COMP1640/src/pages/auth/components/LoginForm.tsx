import React from "react";
import { FieldValues, SubmitHandler, useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import { loginSchema } from "../authValidationSchemas";
import Input from "../../../components/CustomInput";
import { useDispatch, useSelector } from "react-redux";
import { clearMessage, login } from "../../../redux/slices/authSlice";
import { AppDispatch, RootState } from "../../../redux/store";
import Loading from "../../../components/loading/Loading";
import { PATHS } from "../../../constants/path";
import { Navigate, useLocation } from "react-router-dom";
import authUtils from "../../../utils/auth";
import Toast from "../../../components/Toast";

const LoginForm = () => {
  const dispatch = useDispatch<AppDispatch>();
  const location = useLocation();
  const { isError, message, isLoading, isLogin, isFirstLogin } = useSelector(
    (state: RootState) => state.auth
  );

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FieldValues>({
    resolver: yupResolver<FieldValues>(loginSchema),
  });

  const onSubmit: SubmitHandler<FieldValues> = async (data) => {
    try {
      await dispatch(login({ password: data?.password, email: data?.email }));
      authUtils.setEmail(data?.email);
    } catch (error: any) {
      console.log(error.message);
    }

    setTimeout(() => {
      dispatch(clearMessage());
    }, 4000);
  };

  return (
    <>
      {isLoading && <Loading />}
      {isFirstLogin ? (
        <Navigate
          to={{ pathname: `/${PATHS.AUTH.IDENTITY}/${PATHS.AUTH.CHANGE_PASS}` }}
          replace
        />
      ) : (
        isLogin && (
          <Navigate
            to={{
              pathname: location.state?.from || `/${PATHS.HOME.IDENTITY}`,
            }}
          />
        )
      )}
      <div>
        <h1 className="text-2xl font-semibold mb-6">Login</h1>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Input
            register={register}
            errors={errors}
            required
            id="email"
            label="Email"
            type="text"
            placeholder="example@gmail.com"
          />
          <Input
            register={register}
            errors={errors}
            required
            id="password"
            label="Password"
            type="password"
            placeholder="Enter your password"
          />

          {isError && message && (
            <p className="text-red-500 text-xl mb-3 font-bold">{message}</p>
          )}

          <button
            type="submit"
            className="bg-blue-600 text-white p-2 rounded w-full"
          >
            Log in
          </button>
        </form>
      </div>
    </>
  );
};

export default LoginForm;
