import React, { useEffect, useState } from "react";
import useRedux from "../../hooks/useRedux";
import { SelectComponentProps } from "../../types/params.type";
import { useSearchParams } from "react-router-dom";

const RoleSelector = ({ paramName, setParams }: SelectComponentProps) => {
  const { appSelector } = useRedux();
  const [searchParams] = useSearchParams();
  const { role } = appSelector((state) => state.role);
  const [current, setCurrent] = useState<string>();

  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setParams(paramName, e.target.value);
    setParams("page", "1");
  };

  useEffect(() => {
    const param = searchParams.get("role");
    if (param) {
      const temp = role.find((item) => item.name === param);
      setCurrent(temp?.name);
    } else setCurrent("");
  }, [searchParams, role]);

  return (
    <div className="mr-3">
      <label htmlFor="role" className="text-sm text-gray-600">
        Role
      </label>
      <select
        id="role"
        className="block appearance-none w-40 lg:w-60 mt-[2px] h-9 bg-white border border-gray-400 px-2 rounded leading-tight focus:outline-none"
        value={current}
        onChange={(event) => {
          handleChange(event);
        }}
      >
        <option key={"all"} value={""}>
          All
        </option>
        {role?.map((item) => {
          return (
            <option key={item.id} value={item.name}>
              {item.name}
            </option>
          );
        })}
      </select>
    </div>
  );
};

export default RoleSelector;
