// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/** @jsx jsx */

import { ConceptLabels } from '@bfc/shared';
import { jsx } from '@emotion/core';
import { Icon } from 'office-ui-fabric-react/lib/Icon';
import get from 'lodash/get';

import {
  triggerContainerStyle,
  triggerContentStyle,
  titleStyle,
  subtitleStyle,
  triggerIconStyle,
} from './triggerStyles';

function getLabel(data: any): string {
  const labelOverrides = ConceptLabels[data.$type];
  if (labelOverrides) {
    return labelOverrides.subtitle || labelOverrides.title;
  }
  return data.$type;
}

function getName(data: any): string {
  return (
    data.intent || get(data, '$designer.name', ConceptLabels[data.$type] ? ConceptLabels[data.$type].title : data.$type)
  );
}

export const Trigger = ({ data, onClick = () => {} }): JSX.Element => {
  const name = getName(data);
  const label = getLabel(data);

  return (
    <div css={triggerContainerStyle}>
      <div css={triggerContentStyle}>
        <div css={titleStyle}>
          <Icon iconName="Flow" style={triggerIconStyle} />
          <span>{name}</span>
        </div>
        <div css={subtitleStyle}>{label}</div>
      </div>
    </div>
  );
};
