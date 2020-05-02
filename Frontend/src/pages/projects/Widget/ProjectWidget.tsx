import React from "react";
import ProjectCard from "../../../components/ProjectCard/ProjectCard";
import { Fragments } from "../../../api/fragments";
import { useQuery, gql } from "@apollo/client";
import { RouteComponentProps } from "react-router-dom";
import { IProject } from "../../../api/types";
import Loader from "../../../components/Loader/Loader";
import { Alert } from "../../../components/Alert/Alert";
import styles from "./ProjectWidget.module.scss";

const GET_PROJECT = gql`
    query GetProject($id: ID!) {
        project(id: $id) {
            ${Fragments.projectDetail}
        }
    }
`;

type TParams = {
  slug: string,
  projectId: string
}

const ProjectWidgetPage = ({ match } : RouteComponentProps<TParams>): any => {
    const { data, loading, error } = useQuery(
        GET_PROJECT,
        {
            variables: {
                id: match.params.projectId
            }
        }
    );
    const project: IProject | undefined = data?.project;
    return <div className={styles.projectWidget}>
        <Alert type="error" text={error?.message} />
        { loading ? <Loader /> : null }
        { project ? <ProjectCard target="_blank" project={project} /> : null }
    </div>
}

export default ProjectWidgetPage;