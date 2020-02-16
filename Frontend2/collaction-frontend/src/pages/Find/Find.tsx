import React from 'react';
import {useQuery} from '@apollo/react-hooks';
import {gql} from 'apollo-boost';
import {Helmet} from 'react-helmet';
import { Fragments } from '../../api/fragments';
import { IProject } from '../../api/types';

const FindPage = () => {
  const { loading, error, data } = useQuery(FIND_ALL_PROJECTS);

  if(loading) {
    return (<div>Loading</div>);
  }

  if(error) {
    console.error(error);
    return null;
  }

  return (
    <div className="FindPage">
      <Helmet>
        <title>Find Project</title>
        <meta name="description" content="Find project" />
      </Helmet>
      {data.projects.map((project: IProject, index: number) => <div key={index}>{project.name}</div>)}
    </div>
  );
};

export default FindPage;

const FIND_ALL_PROJECTS = gql`
  query FindProjects {
    projects {
      ...ProjectDetail
    }
  }
  ${Fragments.projectDetail}
`;