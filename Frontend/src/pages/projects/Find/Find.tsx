import React, { useState } from 'react';
import {Helmet} from 'react-helmet';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { ProjectStatusFilter } from '../../../api/types';

import { Banner } from '../../../components/Banner/Banner';
import { Section } from '../../../components/Section/Section';
import ProjectsList from '../../../components/ProjectsList/ProjectsList';

import styles from "./Find.module.scss";
import Utils from '../../../utils';
import { useSettings } from '../../../providers/SettingsProvider';

const FindPage = () => {
  const [category, setCategory] = useState("");
  const [status, setStatus] = useState(ProjectStatusFilter.Open);
  const { categories } = useSettings();

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
      <Banner>
        <Section className={styles.banner}>
          <div className={styles.filter}>
            <span>Show me</span>

            <div className={styles.selectWrapper}>
              <select value={category} onChange={handleCategoryChange}>
                <option value="">All</option>
                { categories.map(category => (
                      <option key={category} value={category}>
                        {Utils.formatCategory(category)}
                      </option>
                    ))
                } 
              </select>
              <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
            </div>

            <span>projects which are</span>

            <div className={styles.selectWrapper}>
              <select value={status} onChange={handleStatusChange}>
                <option value={ProjectStatusFilter.Open}>Open</option>
                <option value={ProjectStatusFilter.Closed}>Closed</option>
                <option value={ProjectStatusFilter.ComingSoon}>
                  Coming soon
                </option>
              </select>
              <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
            </div>
          </div>
        </Section>
      </Banner>
      <Section>
        <ProjectsList category={category} status={status} />
      </Section>
    </div>
  );
};

export default FindPage;