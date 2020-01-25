import React from "react";
import { Router } from "@reach/router";
import Layout from "../components/Layout";
import gql from "graphql-tag";
import Loader from "../components/Loader";
import { useQuery } from "react-apollo";
import { navigate } from "gatsby";

const ProjectDetails = () => {
  return (
    <Router>
      <ProjectDetailsPage path="projects/:slug/:projectId"></ProjectDetailsPage>
    </Router>
  );
};

const ProjectDetailsPage = ({ projectId }) => {
  const query = useQuery(GET_PROJECT, { variables: { id: projectId } });

  const { data, loading } = query;

  if (loading) {
    return <Loader />;
  }

  if (!data) {
    navigate("/404");
  }

  return <Layout>{JSON.stringify(data)}!</Layout>;
};

const GET_PROJECT = gql`
  query GetProject($id: ID) {
    project(id: $id) {
      id
      name
      description
      categories {
        category
      }
      bannerImage {
        url
      }
      descriptiveImage {
        url
      }
      goal
      end
      target
      proposal
      remainingTime
      totalParticipants
      percentage
      isActive
      isComingSoon
      isClosed
      isSuccessfull
      isFailed
    }
  }
`;

export default ProjectDetails;
