import React, { useState } from 'react';
import {useQuery} from '@apollo/react-hooks';
import {gql} from 'apollo-boost';
import {Helmet} from 'react-helmet';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ProjectStatusFilter } from '../../api/types';

import { Banner } from '../../components/Banner';
import Loader from '../../components/Loader';
import { Section } from '../../components/Section';
import ProjectsList from '../../components/ProjectsList';

import styles from "./Find.module.scss";
import Utils from '../../utils';

const FindPage = () => {
  const [category, setCategory] = useState("");
  const [status, setStatus] = useState(ProjectStatusFilter.Active);
  const { error, data, loading } = useQuery(GET_CATEGORIES);

  const handleCategoryChange = (e: React.ChangeEvent) => {
    setCategory((e.target as any).value.toString());
  };

  const handleStatusChange = (e: React.ChangeEvent) => {
    setStatus((e.target as any).value);
  };

  return (
    <div className="FindPage">
      <Helmet>
        <title>Find Project</title>
        <meta name="description" content="Find project" />
      </Helmet>
      <Banner dots={true}>
        <Section>
          {loading ? (
            <Loader />
          ) : (
            <div className={styles.filter}>
              <span>Show me</span>

              <div className={styles.selectWrapper}>
                <select value={category} onChange={handleCategoryChange}>
                  <option value="">All</option>
                  {data
                    ? data.__type.enumValues.map((v: any) => (
                        <option key={v.name} value={v.name}>
                          {Utils.formatCategory(v.name)}
                        </option>
                      ))
                    : null}
                </select>
                <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
              </div>

              <span>projects which are</span>

              <div className={styles.selectWrapper}>
                <select value={status} onChange={handleStatusChange}>
                  <option value={ProjectStatusFilter.Active}>Open</option>
                  <option value={ProjectStatusFilter.Closed}>Closed</option>
                  <option value={ProjectStatusFilter.ComingSoon}>
                    Coming soon
                  </option>
                </select>
                <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
              </div>
            </div>
          )}
        </Section>
      </Banner>
      <Section>
        <ProjectsList category={category} status={status} />
      </Section>
    </div>
  );
};

export default FindPage;

const GET_CATEGORIES = gql`
  query {
    __type(name: "Category") {
      enumValues {
        name
      }
    }
  }
`;
