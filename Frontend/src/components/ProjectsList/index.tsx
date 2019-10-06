import React, { Fragment } from "react";
import { useQuery } from "@apollo/react-hooks";
import Card from "../Card";
import gql from "graphql-tag";

export default ({ categoryId }) => {
  const query = categoryId
    ? useQuery(FIND_PROJECTS, {
        variables: { categoryId },
      })
    : useQuery(GET_PROJECTS);

  const { data, loading, error } = query;

  if (loading) {
    return <div>Loading projects...</div>;
  }

  if (error) {
    console.error(error);
    return;
  }

  return (
    <Fragment>
      {data.projects && data.projects.length ? (
        data.projects.map((project, index) => (
          <Card key={index} project={project}></Card>
        ))
      ) : (
        <div>No projects here yet.</div>
      )}
    </Fragment>
  );
};

const GET_PROJECTS = gql`
  query {
    projects {
      id
      name
      url
    }
  }
`;

const FIND_PROJECTS = gql`
  query FindProjects($categoryId: [String]) {
    projects(
      where: { path: "categoryId", comparison: equal, value: $categoryId }
    ) {
      id
      name
      url
    }
  }
`;
