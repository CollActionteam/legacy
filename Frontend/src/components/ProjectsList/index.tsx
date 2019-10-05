import React, { Fragment } from "react";
import { useQuery } from "@apollo/react-hooks";
import gql from "graphql-tag";

export default function ProjectsList() {
  const { data, loading, error } = useQuery(GET_PROJECTS);
  if (loading) {
    return <div>Loading</div>;
  }

  if (error) {
    return <p>ERROR</p>;
  }

  return (
    <Fragment>
      {data.projects &&
        data.projects.map((project, index) => (
          <div key={index}>{project.name}</div>
        ))}
    </Fragment>
  );
}

const GET_PROJECTS = gql`
  query {
    projects {
      id
      name
      url
    }
  }
`;
