import React from 'react';
import { EmailIcon, EmailShareButton, FacebookIcon, FacebookShareButton, FacebookShareCount, LinkedinIcon, LinkedinShareButton, TumblrIcon, TumblrShareButton, TumblrShareCount, TwitterIcon, TwitterShareButton, WhatsappIcon, WhatsappShareButton } from 'react-share';
import { ICrowdaction } from '../../../api/types';

import styles from './SocialMediaSharing.module.scss';

const SocialMediaSharing = ({ crowdaction }: any) => {

  const action = crowdaction as ICrowdaction;  
  const shareUrl = window.location.href;

  return (
    <div className={styles.shareButtons}>
      <FacebookShareButton quote={`${action.name}: ${action.proposal}`} hashtag={action.tags.map(tag => `#${tag.tag.name}`).join(' ')} url={shareUrl}>
        <FacebookIcon size={32} round />
      </FacebookShareButton>
      <FacebookShareCount url={shareUrl}>
        {count => count}
      </FacebookShareCount>

      <TwitterShareButton url={shareUrl} title={action.name} hashtags={action.tags.map(tag => `#${tag.tag.name}`)}>
        <TwitterIcon size={32} round />
      </TwitterShareButton>

      <LinkedinShareButton url={shareUrl} title={action.name} summary={action.proposal}>
        <LinkedinIcon size={32} round />
      </LinkedinShareButton>

      <TumblrShareButton url={shareUrl} title={action.name} caption={action.proposal} tags={action.tags.map(tag => tag.tag.name)}>
        <TumblrIcon size={32} round />        
      </TumblrShareButton>
      <TumblrShareCount url={shareUrl}>
        {count => count}
      </TumblrShareCount>

      <EmailShareButton url={shareUrl} subject={action.name} body="Hi! Check out this crowdaction: ">
        <EmailIcon size={32} round />
      </EmailShareButton>

      <WhatsappShareButton url={shareUrl} title={`Hi! Check out this crowdaction: ${action.name}`}>
        <WhatsappIcon size={32} round />
      </WhatsappShareButton>
    </div>
  );
}

export default SocialMediaSharing;
