import React, { useState } from "react";
import ProjectsList from "../ProjectsList";
import { useQuery } from "@apollo/react-hooks";
import gql from "graphql-tag";

export default () => {
  const [category, setCategory] = useState(null);
  const { data, loading } = useQuery(GET_CATEGORIES);

  const handleCategoryChange = (e: React.ChangeEvent) => {
    setCategory((e.target as any).value.toString());
  };

  return (
    <div>
      {loading ? (
        <div>loading</div>
      ) : (
        <select onChange={handleCategoryChange}>
          <option value="">All</option>
          {data.categories.map((c, i) => (
            <option key={i} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      )}
      <ProjectsList categoryId={category} />
    </div>
  );
};

const GET_CATEGORIES = gql`
  query {
    categories {
      id
      name
    }
  }
`;
