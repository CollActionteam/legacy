import React, { useState } from 'react';
import {Helmet} from 'react-helmet';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { CrowdactionStatusFilter } from '../../../api/types';

import { Banner } from '../../../components/Banner/Banner';
import { Section } from '../../../components/Section/Section';
import CrowdactionsList from '../../../components/CrowdactionsList/CrowdactionsList';

import styles from "./Find.module.scss";
import Utils from '../../../utils';
import { useSettings } from '../../../providers/SettingsProvider';
import { useAnalytics } from '../../../providers/AnalyticsProvider';

const FindPage = () => {
  const [category, setCategory] = useState("");
  const [status, setStatus] = useState(CrowdactionStatusFilter.Open);
  const { categories } = useSettings();
  const { sendUserEvent } = useAnalytics();

  const handleCategoryChange = (e: React.ChangeEvent) => {
    setCategory((e.target as any).value.toString());
    sendUserEvent(false, 'crowdaction', 'apply filter', (e.target as any).value.toString(), null);
  };

  const handleStatusChange = (e: React.ChangeEvent) => {
    setStatus((e.target as any).value);
    sendUserEvent(false, 'crowdaction', 'apply filter', (e.target as any).value.toString(), null);
  };

  return (
    <div className="FindPage">
      <Helmet>
        <title>Find Crowdaction</title>
        <meta name="description" content="Find crowdaction" />
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

            <span>crowdactions which are</span>

            <div className={styles.selectWrapper}>
              <select value={status} onChange={handleStatusChange}>
                <option value={CrowdactionStatusFilter.Open}>Open</option>
                <option value={CrowdactionStatusFilter.Closed}>Closed</option>
                <option value={CrowdactionStatusFilter.ComingSoon}>
                  Coming soon
                </option>
              </select>
              <FontAwesomeIcon icon="angle-down"></FontAwesomeIcon>
            </div>
          </div>
        </Section>
      </Banner>
      <Section>
        <CrowdactionsList category={category} status={status} />
      </Section>
    </div>
  );
};

export default FindPage;